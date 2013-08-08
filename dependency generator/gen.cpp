#include <cstdio>
#include <cstdlib>
#include <ctime>
#include <iostream>
#include <random>
#include <array>
#include <functional>
#include <algorithm>
#include <iomanip>
using namespace std;

int main()
{
    /*std::array<int, std::mt19937::state_size> seed_data;
    std::random_device r;
    std::generate_n(seed_data.data(), seed_data.size(), std::ref(r));
    std::seed_seq seq(std::begin(seed_data), std::end(seed_data));*/
    mt19937 mt(time(NULL));
    uniform_real_distribution<double> realDist(-0.12153268,0.16153268),airincr0(0.14,0.18),airincr1(0.11,0.16),airincr2(0.15,0.23);
    uniform_real_distribution<double> windDist(0,5);
    normal_distribution<double> nwindDist(1,2.1);
    double airTemp[3] = {30.41,30.13,30.57};
    double waterTemp = 26.84;
    double windSpeed[3] = {0};
    int incrementingRange = 2;
    cout<<"AirTemp1,AirTemp2,AirTemp3,WTemp2,WindSpeed1,WindSpeed2,WindSpeed3";
    cout<<setprecision(2)<<fixed;
    double posWindSp = 0;
    for(int i=0; i< 10000;i++)
    {
        posWindSp = (windSpeed[2]+nwindDist(mt));
        posWindSp = (posWindSp < 0) ? 0 : posWindSp;
        cout<<airTemp[0]+realDist(mt)<<","<<airTemp[1]+realDist(mt)<<","<<airTemp[2]+realDist(mt)<<",";
        cout<<waterTemp+realDist(mt)<<",";
        cout<<windSpeed[0]+windDist(mt)<<","<<windSpeed[1]+windDist(mt)<<","<<posWindSp<<"\n";
        if(i >= incrementingRange)
        {
            airTemp[0] += airincr0(mt);
            airTemp[1] += airincr1(mt);
            airTemp[2] += airincr2(mt);
            waterTemp += 0.1;
            incrementingRange *= 1.5;
        }
    }
    return 0;
}
