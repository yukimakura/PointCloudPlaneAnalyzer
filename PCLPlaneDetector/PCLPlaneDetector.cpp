#include "pch.h"
#include <string>
#include "dllExport.h"
#include <pcl/segmentation/sac_segmentation.h>
#include <pcl/filters/extract_indices.h>



DLLEXPORT void __stdcall CalcPlane(float* points, int elementCount, float* returnData,int* returnDataCount,char* msg)
{
	//入力点群
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

	//点群をセットする処理．push_backとかでやってください

	//平面方程式と平面と検出された点のインデックス
	pcl::ModelCoefficients::Ptr coefficients(new pcl::ModelCoefficients);
	pcl::PointIndices::Ptr inliers(new pcl::PointIndices);

	//RANSACによる検出．
	pcl::SACSegmentation<pcl::PointXYZ> seg;
	seg.setOptimizeCoefficients(true); //外れ値の存在を前提とし最適化を行う
	seg.setModelType(pcl::SACMODEL_PLANE); //モードを平面検出に設定
	seg.setMethodType(pcl::SAC_RANSAC); //検出方法をRANSACに設定
	seg.setDistanceThreshold(0.005); //しきい値を設定
	seg.setInputCloud(input_pointcloud.makeShared()); //入力点群をセット
	seg.segment(*inliers, *coefficients); //検出を行う

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