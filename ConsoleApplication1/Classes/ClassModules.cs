using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules
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
            double fa, ex, r0;


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
                        ex = Math.Exp(-Math.Min(60.0, k * t_step));
                        r0 = ( (fmin * t_step) + (f0 - fmin) / k * (1.0 - ex) - q1 ) / (fmin + (f0 - fmin) * ex);
                        t_step = t_step - r0;
                        if (Math.Abs(r0) <= 0.0001 * dt)
                        {
                            break;
                        }
                    }
                }
            }

            // Infiltration capacity is recovering
            else if (r > 0.0)
            {
                r0 = Math.Exp(-r * dt);
                t_step = 1.0 - Math.Exp(-k * t_step);
                t_step = -Math.Log(1.0 - r0 * t_step) / k;
            }

        }

    }

    class WaterShed
    {
        public String SWSID;
        public double impArea;
        public double impMaxDepth;
        public double impInfilt;

        public double perMaxDepth;
        public Horton hInf;
        public double impDstor; // Available impervious depression storage
        public double perDstor;

        public float dryDays;

        public void InitWatershed(double f0, double fmin, double k)
        {
            // Create a new infiltration object
            this.hInf = new Horton();

            // Assign infiltration parameters
            this.hInf.t_step = 0;
            this.hInf.f0 = f0 / 12.0 / 3600.0;
            this.hInf.fmin = f0 / 12.0 / 3600.0;

            // Initialize maximum depression storage
            this.perDstor = this.perMaxDepth;
            this.impDstor = this.impMaxDepth;

            // Unbounded maximum infiltration volume
            this.hInf.fmax = 0;

            // Recalculate on a 10 minute timestep
            this.hInf.dt = 600;

            // Regeneration coefficients, consitent with SWMM
            this.hInf.r = -Math.Log(1 - 0.98); //98% dry along the Horton Infiltration Curve
            this.hInf.r = this.hInf.r / 7; // Converting to units of 1/day
            this.hInf.r = this.hInf.r / 24; // Converting to 1/hour
            this.hInf.r = this.hInf.r / 3600; // Converting to 1/sec
        }

        public void CalculateImperviousRunoff(double RainfallDepth, double ET)
        {
            // Calculates imperv depression storage less ET if dry timestep. (sic)
            this.impDstor = Math.Min(this.impMaxDepth, this.impDstor + (ET / 24.0));

            // Calculate runoff for imperv and perv conditions
            double ImpRunoff = Math.Max(0, RainfallDepth - this.impDstor - this.impInfilt);

            // Update impervious depression storage
            this.impDstor = Math.Max(0, Math.Min(this.impMaxDepth, this.impDstor + this.impInfilt - RainfallDepth));
        }

        public void CalculatePerviousRunoff(double RainfallDepth, double ET)
        {
            double infilt;

            // Remove ET from net rainfall for consistency with SWMM.
            RainfallDepth = Math.Max(0, RainfallDepth);

            // Convert rainfall rate to infiltraiton timestep.
            RainfallDepth = RainfallDepth / 3600.0 / this.hInf.dt;

            // Calculate infiltration rate at current timestep using the Horton infiltration method.
            double perRunoff = 0;
            for (int i = 0; i <= (int)(3600.0 / this.hInf.dt); i = i + 1)
            {
                // Calculate current Horton infiltration rate.
                // Convert to feet/sec from in/timestep.
                this.hInf.Rain(RainfallDepth / 12 / hInf.dt, (this.perMaxDepth - this.perDstor) / 12);

                // Get Horton infiltration at end of timestep.
                // Convert from feet/sec to in/timestep.
                infilt = this.hInf.q * 12.0 * this.hInf.dt;

                // Calculate runoff for pervious condition.
                perRunoff += Math.Max(0, RainfallDepth - this.perDstor - infilt);



                // Update pervious depression storage.
                // Me.pDstor = WorksheetFunction.Max(0, WorksheetFunction.Min(Me.pMaxDepth, Me.pDstor + (infilt - RainfallDepth) + ET / 24))
                // Modify ET for fine step
                // Me.pDstor = WorksheetFunction.Max(0, WorksheetFunction.Min(Me.pMaxDepth, Me.pDstor + (infilt - RainfallDepth) + ET / 24 / (3600 / Me.hInf.dt)+ CalculatePerviousRunoff) )
                this.perDstor = Math.Max(0, Math.Min(this.perMaxDepth, this.perDstor + (infilt - RainfallDepth + (ET / 24.0 / (3600.0 / this.hInf.dt)))));
            }
        }
    }
}
