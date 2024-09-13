# What is the NECN Succession Extension?

The NECN Succession extension was designed to provide total ecosystem accounting of Carbon and Nitrogen and to allow species to respond dynamically to a changing climate via establishment and growth.  NECN calculates how cohorts grow, reproduce, age, and die.  Dead biomass is tracked over time, divided into four pools:  surface wood, soil wood (dead coarse roots), surface litter (dead leaves), and soil litter (dead fine roots).  In addition, three principle soil pools:  fast (soil organic matter (SOM) 1), slow (SOM2), and passive (SOM3) are simulated, following the Century soil model.

# Recent Additions

- [x] New with v8
  - [ ] Species and Functional Group tables have been merged
- [x] New with v7
  - [ ] Integrated drought mortality via an optional table that defines statistical relationships
  - [ ] Updated soil water algorithms that improves low moisture estimates
  - [ ] New continuous function for estimating regeneration, given light, that requires fewer parameters
  - [ ] Optionally capacity to modify PET according to topography
  - [ ] Establishment can now optionally be reduced according to climatic water deficit and/or soil drainage
  - [ ] Growth as a function of soil water now has an optional functional form that is more flexible, better able to account for wet soils
- [x] New with v6.7 - simulates grass species and their interactions with regeneration.

# Standard Features

- [x] Estimate Net Ecosystem Exchange.
- [x] Estimate Actual and Potential Evapotranspiration, Climatic Water Deficit, and Available Soil Water.
- [x] Estimate smoke emissions from wildfires and prescribed fires.
- [x] Species parameters input as CSV delimited files.
- [x] Estimate Soil Organic Carbon, Soil Nitrogen, Nitrogen fluxes

# Citation

Scheller, R.M., D. Hua, P.V. Bolstad, R. Birdsey, D.J. Mladenoff. 2011. The effects of forest harvest intensity in combination with wind disturbance on carbon dynamics in a Lake States mesic landscape. Ecological Modelling 222: 144-153.

# Release Notes

- Latest official release: Version 8 — August 2023
- [NECN User Guide](https://github.com/LANDIS-II-Foundation/Extension-NECN-Succession/blob/master/docs/LANDIS-II%20Net%20Ecosystem%20CN%20Succession%20v8.0%20User%20Guide.pdf).
- [User Guide for Climate Library](https://github.com/LANDIS-II-Foundation/Library-Climate/blob/master/docs/LANDIS-II%20Climate%20Library%20v4.2%20User%20Guide.pdf)
- Full release details found in the NECN User Guide and on GitHub.
- This extension was formerly named Century Succession.

# Requirements

To use NECN, you need:

- The [LANDIS-II model v8.0](http://www.landis-ii.org/install) installed on your computer.
- Example files (see below)

# Download the Extension


The latest version can be downloaded [here](https://github.com/LANDIS-II-Foundation/Extension-NECN-Succession/blob/master/deploy/installer/LANDIS-II-V8%20NECN%20Succession%208.0-setup.exe). (Look for the download icon in the upper right corner.) Launch the installer.


# Example Files

LANDIS-II requires a global parameter file for your scenario, and then different parameter files for each extension.

Landscape example files are [here](https://downgit.github.io/#/home?url=https://github.com/LANDIS-II-Foundation/Extension-NECN-Succession/tree/master/testing/Core8-NECN8-Landscape).

Single-cell example files are [here](https://downgit.github.io/#/home?url=https://github.com/LANDIS-II-Foundation/Extension-NECN-Succession/tree/master/testing/Core8-NECN8-SingleCell).


# Support

If you have a question, please contact Robert Scheller. 
You can also ask for help in the [LANDIS-II users group](http://www.landis-ii.org/users).

If you come across any issue or suspected bug when using NECN, please post about it in the [issue section of the Github repository](https://github.com/LANDIS-II-Foundation/Extension-NECN-Succession/issues).

# Author

[Robert Scheller](https://sites.google.com/a/ncsu.edu/dynamic-ecosystems-landscape-lab/people/robert-scheller)




