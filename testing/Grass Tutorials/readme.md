# Description
Author: Chihiro Haga (chihiro.haga@ge.see.eng.osaka-u.ac.jp)
Date: 2023.06.06
Related publication: https://doi.org/10.1016/j.ecolmodel.2022.110072

This tutorial will demonstrate how herbaceous species alter forest succession using the NECN succession extension.

# Scenarios to be simulated
- grassb_0: Start from bare land without dwarf-bamboo
- grassb_1000: Start from dense dwarf-bamboo land (AGB = 1kg/m2)

# How to run simulations?
1. Install the latest LANDIS-II-core (v7), NECN-succession (v6.10.1), and Output-biomass (v3.0) extensions using .exe files in the installers folder.
2. Open ./output/grassb_0 or ./output/grassb_1000 in the file explorer
3. Double click the start-simulation.bat file

# Which output files to be checked after the simulation?
- spp-biomass-log.csv: Total aboveground biomass (AGB) of each species
