#library("raster")

#args <- commandArgs(trailingOnly = TRUE)

#print(args[1])
#setwd(args[1])


#cwd_directory <- args[2]
#print(args[2])

#TODO get this to work with a batch script

setwd("C:/Users/Sam/Documents/Research/Extension-NECN-Succession/testing/Core7-NECN6.9-SingleCellCopy")
cwd_directory <- "./NECN/"
cwd_rasters <- list.files(cwd_directory, pattern = "CWD", full.names = TRUE)[1:15] #get first 30 years of CWD rasters
#print(cwd_rasters)
raster_test <- raster::raster(cwd_rasters[1])
#print(raster_test)
cwd_stack <- raster::stack(cwd_rasters)
#print(cwd_stack)

mean_cwd <- raster::calc(cwd_stack, mean) #average value

#print(raster::values(mean_cwd))

raster::writeRaster(mean_cwd, filename = paste0(getwd(), "/CWD_normals.tif"),
					overwrite = TRUE)

rm(cwd_stack)
rm(mean_cwd)


setwd("C:/Users/Sam/Documents/Research/Extension-NECN-Succession/testing/Core7-NECN6.9-SingleCellCopy")
swa_directory <- "./NECN/"
swa_rasters <- list.files(swa_directory, pattern = "SWA", full.names = TRUE)[1:15] #get first 30 years of CWD rasters
#print(swa_rasters)
raster_test <- raster::raster(swa_rasters[1])
#print(raster_test)
swa_stack <- raster::stack(swa_rasters)
#print(cwd_stack)

mean_swa <- raster::calc(swa_stack, mean) #average value

#print(raster::values(mean_cwd))

raster::writeRaster(mean_swa, filename = paste0(getwd(), "/SWA_normals.tif"),
					overwrite = TRUE)
            