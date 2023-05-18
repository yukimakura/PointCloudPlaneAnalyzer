#include "pch.h"
#include <string>
#include "dllExport.h"
#include <pcl/segmentation/sac_segmentation.h>
#include <pcl/filters/extract_indices.h>
#include <pcl/point_cloud.h>
#include <pcl/point_types.h>
#include <pcl/common/transforms.h>




typedef struct _StructPoint3D {
	float X;
	float Y;
	float Z;
}StructPoint3D;

double degToRad(double deg)
{
	double rad = deg / 180.0 * M_PI;
	rad = atan2(sin(rad), cos(rad));
	return rad;
}

DLLEXPORT void __stdcall CalcPlaneStruct(StructPoint3D* points, int elementCount, StructPoint3D* returnData, int* returnDataCount,float errorMm)
{
	//���͓_�Q
	pcl::PointCloud<pcl::PointXYZ> input_pointcloud;
	
	for (size_t i = 0; i < elementCount; i++)
	{
		input_pointcloud.push_back(pcl::PointXYZ(points[i].X, points[i].Y, points[i].Z));
	}


	//���ʕ������ƕ��ʂƌ��o���ꂽ�_�̃C���f�b�N�X
	pcl::ModelCoefficients::Ptr coefficients(new pcl::ModelCoefficients);
	pcl::PointIndices::Ptr inliers(new pcl::PointIndices);

	//RANSAC�ɂ�錟�o�D
	pcl::SACSegmentation<pcl::PointXYZ> seg;
	seg.setOptimizeCoefficients(true); //�O��l�̑��݂�O��Ƃ��œK�����s��
	seg.setModelType(pcl::SACMODEL_PLANE); //���[�h�𕽖ʌ��o�ɐݒ�
	seg.setMethodType(pcl::SAC_RANSAC); //���o���@��RANSAC�ɐݒ�
	seg.setDistanceThreshold(errorMm); //�������l��ݒ�
	seg.setInputCloud(input_pointcloud.makeShared()); //���͓_�Q���Z�b�g
	seg.segment(*inliers, *coefficients); //���o���s��

	pcl::PointCloud<pcl::PointXYZ> plane_pointcloud;
	pcl::ExtractIndices<pcl::PointXYZ> extract;
	extract.setInputCloud(input_pointcloud.makeShared());
	extract.setIndices(inliers);
	extract.setNegative(false);
	extract.filter(plane_pointcloud);

	(*returnDataCount) = plane_pointcloud.size();

	for (size_t i = 0; i < (*returnDataCount); i++)
	{
		returnData[i] = StructPoint3D{ plane_pointcloud[i].x, plane_pointcloud[i].y, plane_pointcloud[i].z };
	}

}

DLLEXPORT void __stdcall TransformPointCloud(StructPoint3D* points, int elementCount, StructPoint3D* returnData, int* returnDataCount,float roll,float pitch,float yaw,float x,float y,float z)
{
	//���͓_�Q
	pcl::PointCloud<pcl::PointXYZ> inputPointcloud;

	for (size_t i = 0; i < elementCount; i++)
	{
		inputPointcloud.push_back(pcl::PointXYZ(points[i].X, points[i].Y, points[i].Z));
	}

	pcl::PointCloud<pcl::PointXYZ> rotatedPointcloud;
	Eigen::Affine3f tf = pcl::getTransformation(x, y, z, degToRad(roll), degToRad(pitch), degToRad(yaw));
	pcl::transformPointCloud(inputPointcloud, rotatedPointcloud, tf,true);

	(*returnDataCount) = rotatedPointcloud.size();

	for (size_t i = 0; i < (*returnDataCount); i++)
	{
		returnData[i] = StructPoint3D{ rotatedPointcloud[i].x, rotatedPointcloud[i].y, rotatedPointcloud[i].z };
	}
}
