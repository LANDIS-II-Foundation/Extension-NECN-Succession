//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

using Landis.Library.LeafBiomassCohorts;  

namespace Landis.Extension.Succession.NECN_Hydro.AgeOnlyDisturbances
{
    /// <summary>
    /// The handlers for various type of events related to age-only
    /// disturbances.
    /// </summary>
    public static class Events
    {
        public static void CohortTotalMortality(object         sender,
                                      Landis.Library.BiomassCohorts.DeathEventArgs eventArgs)
        {
            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            
            PoolPercentages cohortReductions = Module.Parameters.CohortReductions[disturbanceType];

            ICohort cohort = (Landis.Library.LeafBiomassCohorts.ICohort) eventArgs.Cohort;
            ActiveSite site = eventArgs.Site;
            float foliar = cohort.LeafBiomass; 
            float wood = cohort.WoodBiomass; 
            

            float foliarInput = ReduceInput(foliar, cohortReductions.Foliar, site);
            float woodInput   = ReduceInput(wood, cohortReductions.Wood, site);

            //PlugIn.ModelCore.UI.WriteLine("EVENT: Cohort Died: species={0}, age={1}, disturbance={2}.", cohort.Species.Name, cohort.Age, eventArgs.DisturbanceType);
            //PlugIn.ModelCore.UI.WriteLine("       Cohort Reductions:  Foliar={0:0.00}.  Wood={1:0.00}.", cohortReductions.Foliar, cohortReductions.Wood);
            //PlugIn.ModelCore.UI.WriteLine("       InputB/TotalB:  Foliar={0:0.00}/{1:0.00}, Wood={2:0.0}/{3:0.0}.", foliarInput, foliar, woodInput, wood);
            
            ForestFloor.AddWoodLitter(woodInput, cohort.Species, site);
            ForestFloor.AddFoliageLitter(foliarInput, cohort.Species, site);
            
            Roots.AddCoarseRootLitter(wood, cohort, cohort.Species, site);  // All of cohorts roots are killed.
            Roots.AddFineRootLitter(foliar, cohort, cohort.Species, site);
            
        }

        //---------------------------------------------------------------------

        public static float ReduceInput(float     poolInput,
                                          Percentage reductionPercentage,
                                          ActiveSite site)
        {
            float reduction = (poolInput * (float) reductionPercentage);
            
            SiteVars.SourceSink[site].Carbon        += (double) reduction * 0.47;
            //SiteVars.FireEfflux[site]               += (double) reduction * 0.47;
            
            return (poolInput - reduction);
        }

        //---------------------------------------------------------------------

        public static void SiteDisturbed(object               sender,
                                         Landis.Library.BiomassCohorts.DisturbanceEventArgs eventArgs)
        {

            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            
            if(disturbanceType.ToString() == "disturbance:fire")
                return;

            PoolPercentages poolReductions = Module.Parameters.PoolReductions[disturbanceType];

            ActiveSite site = eventArgs.Site;
            SiteVars.SurfaceDeadWood[site].ReduceMass(poolReductions.Wood);
            SiteVars.SurfaceStructural[site].ReduceMass(poolReductions.Foliar);
            SiteVars.SurfaceMetabolic[site].ReduceMass(poolReductions.Foliar);
        }
    }
}
