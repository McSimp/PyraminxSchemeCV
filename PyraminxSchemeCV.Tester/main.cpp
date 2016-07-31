#include <iostream>
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <memory>
#include <chrono>
#include <cv.h>

#define SHOW_DEBUG 1

// DEBUGGING
//int minSlider = (int)CANNY_THRESH_MIN;
//int maxSlider = (int)CANNY_THRESH_MAX;
//int epsSlider = (int)(EPISILON_PERCENTAGE * 100);

int main()
{
#if SHOW_DEBUG
	std::cout << "Hello world!" << std::endl;

	//cv::namedWindow("imcanny", cv::WINDOW_AUTOSIZE);
	//cv::namedWindow("polys", cv::WINDOW_AUTOSIZE);

	//cv::createTrackbar("canny min", "imcanny", &minSlider, 255, OnCannyMinTrackbar);
	//cv::createTrackbar("canny max", "imcanny", &maxSlider, 255, OnCannyMaxTrackbar);
	//cv::createTrackbar("epsilon percentage", "polys", &epsSlider, 100, OnEpisilonTrackbar);
#endif
	cv::VideoCapture cap(0);
	cv::Mat frame;

	ResetColourCounts();

	while (true)
	{
		cap.read(frame);
		cv::imshow("camera", frame);
		auto start = std::chrono::steady_clock::now();
		cv::Mat flipped;
		cv::flip(frame, flipped, 1);
		cv::Mat imsize;
		cv::resize(flipped, imsize, cv::Size(0, 0), 0.8, 0.8);
		Face face;
		if (ProcessFrameAndColours(imsize, face, false))
		{
			// yay
		}

		auto colourCounts = GetColourCounts();
		std::cout << "========" << std::endl;
		for (int i = 0; i < 9; i++)
		{
			auto max = std::max_element(std::begin(colourCounts[i]), std::end(colourCounts[i]));
			TriColour colour = (TriColour)std::distance(std::begin(colourCounts[i]), max);
			std::cout << GetColourName(colour) << " - " << colourCounts[i][colour] << std::endl;
		}

		auto end = std::chrono::steady_clock::now();
		//std::cout << std::chrono::duration <double, std::milli>(end-start).count() << " ms" << std::endl;
		cv::waitKey(1);
	}

	int a;
	std::cin >> a;
	return 0;
}