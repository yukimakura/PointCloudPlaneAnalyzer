#include "pch.h"
#include <string>
#include "dllExport.h"
#include <pcl/segmentation/sac_segmentation.h>
#include <pcl/filters/extract_indices.h>



DLLEXPORT void __stdcall CalcPlane(float* points, int elementCount, float* returnData,int* returnDataCount,char* msg)
{
	//���͓_�Q
	pcl::PointCloud<pcl::PointXYZ> input_pointcloud;
	//
	//std::stringstream ss;
	//ss << "elemcnt:" << elementCount << ",pointsize:" << sizeof(points) / sizeof(float);
	//std::char_traits<char>::copy(msg, ss.str().c_str(), ss.str().size() + 1);

	//std::strcpy(msg, ss.str().c_str());
	//input_pointcloud.push_back(pcl::PointXYZ(points[0], points[1], points[2]));
	for (size_t i = 0; i < elementCount ; i++  )
	{
		input_pointcloud.push_back(pcl::PointXYZ(points[i], points[i+1], points[i+2]));
	}

	//�_�Q���Z�b�g���鏈���Dpush_back�Ƃ��ł���Ă�������

	//���ʕ������ƕ��ʂƌ��o���ꂽ�_�̃C���f�b�N�X
	pcl::ModelCoefficients::Ptr coefficients(new pcl::ModelCoefficients);
	pcl::PointIndices::Ptr inliers(new pcl::PointIndices);

	//RANSAC�ɂ�錟�o�D
	pcl::SACSegmentation<pcl::PointXYZ> seg;
	seg.setOptimizeCoefficients(true); //�O��l�̑��݂�O��Ƃ��œK�����s��
	seg.setModelType(pcl::SACMODEL_PLANE); //���[�h�𕽖ʌ��o�ɐݒ�
	seg.setMethodType(pcl::SAC_RANSAC); //���o���@��RANSAC�ɐݒ�
	seg.setDistanceThreshold(0.005); //�������l��ݒ�
	seg.setInputCloud(input_pointcloud.makeShared()); //���͓_�Q���Z�b�g
	seg.segment(*inliers, *coefficients); //���o���s��

	pcl::PointCloud<pcl::PointXYZ> plane_pointcloud;
	pcl::ExtractIndices<pcl::PointXYZ> extract;
	extract.setInputCloud(input_pointcloud.makeShared());
	extract.setIndices(inliers);
	extract.setNegative(false);
	extract.filter(plane_pointcloud);

	//(*returnDataCount) = input_pointcloud.size();
	(*returnDataCount) = plane_pointcloud.size();
	for (size_t i = 0; i < (*returnDataCount); i++)
	{
		returnData[i*3] = plane_pointcloud[i].x;
		returnData[i*3+1] = plane_pointcloud[i].y;
		returnData[i*3+2] = plane_pointcloud[i].z;
	}

}