using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMOTModules
{
    class BMP
    // For simple representation of a pond BMP as defined by
    // surface area, depth, and background constant infiltration
    {
        public double bmpArea;
        public double bmpMaxDepth;
        public double bmpInfilt;
        public double bmpDepth;

        public void InitBMP()
        {
            bmpDepth = bmpMaxDepth;
        }

        public double CalculateBMPOverflow(double Inflow, double ET)
        {
            // Recalculate inflow as runoff depth into BMP
            Inflow /= bmpArea;

            // Calculate loss due to ET
            bmpDepth = Math.Min(bmpMaxDepth, bmpDepth + (ET / 24.0));

            // Update available BMP storage
            bmpDepth = Math.Max(bmpMaxDepth, bmpMaxDepth - Math.Min(bmpMaxDepth, bmpDepth + bmpInfilt - Inflow));

            // Calculate BPM Overflow
            double BMPOverflow = Math.Max(0, Inflow - bmpDepth - bmpInfilt);

            return BMPOverflow;
        }
    }

    class Horton
    // Used to calculate infiltration rate based on omdified Horton (1933)
    // Mostly based on EPA SWMM infil.c source code
    {

        public double t_step; // Seconds; Current time step
        public double dt; // Seconds; Time step

        public double q; // ft/s; Current inf. rate
        public double fmin; // ft/s; Min inf. rate
        public double fmax; // ft/s; Max inf. rate

        public double k; // (1/sec); Decay coefficient of infiltration rate
        public double r; // (1/sec); Regenration coefficient of infiltration rate
        public double f0; // ft/s;  Initial infiltration rate

        public void Init() // Sets initial time to zero
        {
            t_step = 0;
            dt = 1;
        }

        public void Rain(double rainfall_depth, double pond_depth)
        // Calculates current infiltration rate
        {
            double q0, q1;
            double fa;

            // if no infiltration
            if ( (f0 - fmin < 0.0) || (k < 0.0) || (r < 0.0) )
            {
                q = 0.0;
                return; // Break the method here.
            }

            // Compute available water for infiltration
            fa = rainfall_depth + (pond_depth / dt);

            // Special case for constant infiltration
            if ( (f0 - fmin == 0.0) || (k == 0.0) )
            {
                q = Math.Max(0.0, Math.Min(f0, fa));
                return; // Break the method here.
            }

            // Water available to infiltrate
            if (fa > 0.0)
            {
                // Get average infil rate over current time step
                if (t_step >= (10.0 / k))
                {
                    q0 = (fmin * t_step) + (f0 - fmin) / k;
                    q1 = q0 + fmin * dt;
                }
                // Correct for max infiltration rate, if set
                else
                {
                    q0 = (fmin * t_step) + (f0 - fmin) / k * (1.0 - Math.Exp(-k * t_step));
                    q1 = (fmin * (t_step + dt)) + (f0 - fmin) / k * (1.0 - Math.Exp(-k * (t_step + dt)) );
                }

                // Make sure we dont infiltrate more water than we actually have
                q = Math.Min((q1 - q0) / dt, fa);

                if ( ((t_step + dt) > (10.0 / k)) || (q < fa) )
                {
                    // if q on flat portion of curve or infil is less than 
                    // available water increase time step 
                    t_step = t_step + dt;
                }
                else
                {
                    // Infiltration limited by available capacity
                    // Solving using Newton-Raphson method
                    q1 = q0 + q * dt;
                    t_step = t_step + (dt / 2.0);
                    for (int i = 0; i <= 50; i = i + 1)
                    {
                        double ex = Math.Exp(-Math.Min(60.0, k * t_step));
                        double r0 = ( (fmin * t_step) + (f0 - fmin) / k * (1.0 - ex) - q1 ) / (fmin + (f0 - fmin) * ex);
                        t_step = t_step - r0;
                        if (Math.Abs(r0) <= 0.0001 * dt)
                        {
                            break;
                        }
                    }
                }
            }


        }

    }

    //class WaterShed
    //{
    //    public String SWSID;
    //    public double impArea;
    //    public double impMaxDepth;
    //    public double impInfilt;

    //    public double perMaxDepth;
    //    public horInf 
    //}
}
