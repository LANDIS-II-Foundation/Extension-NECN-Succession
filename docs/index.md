# What is NECN?



It allows any user to dynamically simulate the evolution of the forest road network in the simulated landscape of LANDIS-II. It does so by creating roads to cells that are harvested by a harvest module (such as [Base Harvest](http://www.landis-ii.org/extensions/base-harvest) or [Biomass Harvest](http://www.landis-ii.org/extensions/biomass-harvest)), while reducing the costs of construction of roads as much as possible.


# What can it do ? (Features)


![](https://raw.githubusercontent.com/Klemet/LANDIS-II-Forest-Roads-Simulation-module/master/screenshots/EvolutionOfNetwork.png)

- [x] Build forest roads from recently harvested cells to exit points for the wood
- [x] Compute the path of new forest roads to minimize the costs of construction according to factors such as elevation, water and soils.
- [x] **[Optional]** Create loops in the network to make it more realistic
- [x] **[Optional]** Age the roads and destroy them with time
- [x] **[Optional]** Simulate the wood flux going through the roads, and upgrade their size to accomodate to this flux
- [x] **[Optional]** Take into account repeated cuts to optimize the choice of road types
- [ ] **[Optional]** **[Still to come]** Deactivation and reactivation of forest roads for conservation purposes
- [ ] **[Optional]** **[Still to come]** Estimation of CO2 emissions coming from the usage of the roads

# What do I need to use it ? (Requirements)

To use the FRS module, you need:

- The [LANDIS-II model v7.0](http://www.landis-ii.org/install) installed on your computer.
- The parameter files for your scenario (see Parameterization section below).

# Where do I download it ? (Download)

Version 1.0 can be downloaded [here](https://github.com/Klemet/LANDIS-II-Forest-Roads-Simulation-module/releases/download/1.0/LANDIS-II-V7.Forest.Road.Simulation.module.1.0-setup.exe). To install it on your computer, just launch the installer.

# Where do I get the parameter files ? (Parameterization)

LANDIS-II requires a global parameter file for your scenario, and then different parameter files for each extension that you use.

To know how to generate the parameter files for the succession extension and the harvest extension that you will use, please refer to their user manual.

**To generate the parameter files needed for the FRS module, please read the [user guide of the module](https://raw.githubusercontent.com/Klemet/LANDIS-II-Forest-Roads-Simulation-module/master/LANDIS-II%20Forest%20Roads%20Simulation%20v1.0%20User%20Guide.pdf).** It will help you throught the process in detail !

# Can I test it ? Can I have an example of parameter files ?

Yes, and yes ! Just [download the example files](https://downgit.github.io/#/home?url=https://github.com/Klemet/LANDIS-II-Forest-Roads-Simulation-module/tree/master/Examples), and you'll be set !

To launch the example scenario, you'll need the [Age-Only succession](http://www.landis-ii.org/extensions/age-only-succession) extension and the [Base Harvest](http://www.landis-ii.org/extensions/base-harvest) extension installed on your computer, in addition to the FRS module. Just launch the `test_scenario.bat` file, and the example scenario should run.

# What do I do if I have questions, or if I need help ? (Support)

If you have a question, please send me an e-mail at clem.hardy@outlook.fr. I'll do my best to answer you in time. 
You can also ask for help in the [LANDIS-II users group](http://www.landis-ii.org/users).

If you come across any issue or suspected bug when using the FRS module, please post about it in the [issue section of the Github repository of the module](https://github.com/Klemet/LANDIS-II-Forest-Roads-Simulation-module/issues).

# Author

[Robert Scheller](http://www.cef-cfr.ca/index.php?n=Membres.ClementHardy)

Professor at the North Carolina State University

Mail : rschell@ncsu.edu

Github : [https://github.com/Klemet](https://github.com/Klemet)

