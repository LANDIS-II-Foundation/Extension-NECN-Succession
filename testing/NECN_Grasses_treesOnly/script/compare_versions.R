# Date: 2021.04.21
# Author: Haga, Chihiro
# 
# Title: 
#   Compare grasses v67 & original v67
#     - Grasses v67:  b96c890021b5512891cce01a451f219811a40a91
#     - Original v67: 4582b761987ff074d3d4ec196485bc3d9eda5425
# 
# Summary:
#   For Tree species calculation of Grasses v67 & Original v67,
#   NO difference was found on 
#     1. NECN-succession-log.csv
#     2. NECN-succession-monthly-log.csv
#     3. NECN-prob-establish-log.csv
#     4. NECN-reproduction-log.csv
#     5. NECN-calibrate-log.csv
#         Note: 
#         Four new variables were added to calibrate-log of grasses v67,
#         so you will found difference, but it is normal.


# Library
library(tidyverse)
library(ggthemes)
library(patchwork)

# Initialize variables ----
## Simulation cases ----
test_names <- c('test01_SingleCohort',
                'test02_TwoCohorts_SameSpp',
                'test03_TwoCohorts_DifferentSpp')
## Version names ----
version_names <- c('grasses67', 'original67')
all_cases <- expand.grid(test_names, version_names)
## Log names ----
necnlog_name <- 'NECN-succession-log.csv'
monthlylog_name <- 'NECN-succession-monthly-log.csv'
estlog_name <- 'NECN-prob-establish-log.csv'
reprodlog_name <- 'NECN-reproduction-log.csv'
callog_name <- 'NECN-calibrate-log.csv'


# Define functions ----
ReadLog <- function(test_name, version_name, log_name) {
  read_csv(file.path(paste0(test_name, '_', version_name), 
                     log_name), 
           col_types = cols()) %>% 
    mutate(testname = test_name,
           necnver = version_name)
}


# Main ----
## 1. NECN-succession-log.csv ----
# Read data
necnlog_list <- list(); iter <- 1
for (test_name in test_names) {
  for (version_name in version_names) {
    necnlog_list[[iter]] <- ReadLog(test_name, version_name, necnlog_name)
    iter <- iter + 1
  }
}
# Transform into tidy data
necnlog_df <- bind_rows(necnlog_list) %>% 
  dplyr::select(-X61) %>% 
  pivot_longer(cols = -c("Time", "ClimateRegionName", "ClimateRegionIndex", "NumSites", "testname", "necnver"))
# Plot
plt_test01_necn <- necnlog_df %>% 
  filter(testname == 'test01_SingleCohort') %>% 
  ggplot(aes(x = Time, y = value, group = interaction(testname, necnver, name), color = necnver)) +
  geom_line() + facet_wrap(~name, scales = 'free') + 
  labs(title = 'NECN-succession-log.csv of test01_SingleCohort', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test02_necn <- necnlog_df %>% 
  filter(testname == 'test02_TwoCohorts_SameSpp') %>% 
  ggplot(aes(x = Time, y = value, group = interaction(testname, necnver, name), color = necnver)) +
  geom_line() + facet_wrap(~name, scales = 'free') + 
  labs(title = 'NECN-succession-log.csv of test02_TwoCohorts_SameSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test03_necn <- necnlog_df %>% 
  filter(testname == 'test03_TwoCohorts_DifferentSpp') %>% 
  ggplot(aes(x = Time, y = value, group = interaction(testname, necnver, name), color = necnver)) +
  geom_line() + facet_wrap(~name, scales = 'free') + 
  labs(title = 'NECN-succession-log.csv of test03_TwoCohorts_DifferentSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_necn <- plt_test01_necn / plt_test02_necn / plt_test03_necn
ggsave('fig01_NECN-succession-log.pdf', plt_necn, width = 27, height = 27)

# Compare numeric values
necnlog_diff_df <- necnlog_df %>% 
  pivot_wider(names_from = 'necnver', values_from = 'value') %>% 
  mutate(difference = grasses67 - original67)
summary(necnlog_diff_df$difference)
#    Min. 1st Qu.  Median    Mean 3rd Qu.    Max. 
#       0       0       0       0       0       0



## 2. NECN-monthly-log.csv ----
# Read data
monthlylog_list <- list(); iter <- 1
for (test_name in test_names) {
  for (version_name in version_names) {
    monthlylog_list[[iter]] <- ReadLog(test_name, version_name, monthlylog_name)
    iter <- iter + 1
  }
}
# Transform into tidy data
monthlylog_df <- bind_rows(monthlylog_list) %>% 
  dplyr::select(-X15) %>% 
  pivot_longer(cols = -c("Time", "Month", "ClimateRegionName", "ClimateRegionIndex", "NumSites", "testname", "necnver")) %>% 
  mutate(date = lubridate::ymd(paste(Time + 2015, Month, 1, sep = '-')))
# Plot
plt_test01_monthly <- monthlylog_df %>% 
  filter(testname == 'test01_SingleCohort') %>% 
  ggplot(aes(x = date, y = value, group = interaction(testname, necnver, name), color = necnver)) +
  geom_line(size = .1) + facet_wrap(~name, scales = 'free') + 
  labs(title = 'NECN-monthly-log.csv of test01_SingleCohort', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test02_monthly <- monthlylog_df %>% 
  filter(testname == 'test02_TwoCohorts_SameSpp') %>% 
  ggplot(aes(x = date, y = value, group = interaction(testname, necnver, name), color = necnver)) +
  geom_line(size = .1) + facet_wrap(~name, scales = 'free') + 
  labs(title = 'NECN-monthly-log.csv of test02_TwoCohorts_SameSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test03_monthly <- monthlylog_df %>% 
  filter(testname == 'test03_TwoCohorts_DifferentSpp') %>% 
  ggplot(aes(x = date, y = value, group = interaction(testname, necnver, name), color = necnver)) +
  geom_line(size = .1) + facet_wrap(~name, scales = 'free') + 
  labs(title = 'NECN-monthly-log.csv of test03_TwoCohorts_DifferentSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_monthly <- plt_test01_monthly / plt_test02_monthly / plt_test03_monthly
ggsave('fig02_NECN-monthly-log.pdf', plt_monthly, width = 27, height = 27)

# Compare numeric values
monthlylog_diff_df <- monthlylog_df %>% 
  pivot_wider(names_from = 'necnver', values_from = 'value') %>% 
  mutate(difference = grasses67 - original67)
summary(monthlylog_diff_df$difference)
#    Min. 1st Qu.  Median    Mean 3rd Qu.    Max. 
#       0       0       0       0       0       0


## 3. NECN-establishment-log.csv ----
# Read data
estlog_list <- list(); iter <- 1
for (test_name in test_names) {
  for (version_name in version_names) {
    estlog_list[[iter]] <- ReadLog(test_name, version_name, estlog_name)
    iter <- iter + 1
  }
}
# Transform into tidy data
estlog_df <- bind_rows(estlog_list) %>% 
  dplyr::select(-X12) %>% 
  pivot_longer(cols = -c("Time", "SpeciesName", "ClimateRegion", "testname", "necnver")) %>% 
  filter(!is.nan(value))
# Plot
plt_test01_est <- estlog_df %>% 
  filter(testname == 'test01_SingleCohort') %>% 
  ggplot(aes(x = Time, y = value, group = interaction(testname, necnver, name, SpeciesName), color = necnver)) +
  geom_line() + facet_grid(name~SpeciesName, scales = 'free') + 
  labs(title = 'NECN-establishment-log.csv of test01_SingleCohort', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test02_est <- estlog_df %>% 
  filter(testname == 'test02_TwoCohorts_SameSpp') %>% 
  ggplot(aes(x = Time, y = value, group = interaction(testname, necnver, name, SpeciesName), color = necnver)) +
  geom_line() + facet_grid(name~SpeciesName, scales = 'free') + 
  labs(title = 'NECN-establishment-log.csv of test02_TwoCohorts_SameSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test03_est <- estlog_df %>% 
  filter(testname == 'test03_TwoCohorts_DifferentSpp') %>% 
  ggplot(aes(x = Time, y = value, group = interaction(testname, necnver, name, SpeciesName), color = necnver)) +
  geom_line() + facet_grid(name~SpeciesName, scales = 'free') + 
  labs(title = 'NECN-establishment-log.csv of test03_TwoCohorts_DifferentSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_est <- plt_test01_est / plt_test02_est / plt_test03_est
ggsave('fig03_NECN-establishment-log.pdf', plt_est, width = 27, height = 27)

# Compare numeric values
estlog_diff_df <- estlog_df %>% 
  pivot_wider(names_from = 'necnver', values_from = 'value') %>% 
  mutate(difference = grasses67 - original67)
summary(estlog_diff_df$difference)
#    Min. 1st Qu.  Median    Mean 3rd Qu.    Max. 
#       0       0       0       0       0       0


## 4. NECN-reproduction-log.csv ----
# Read data
reprodlog_list <- list(); iter <- 1
for (test_name in test_names) {
  for (version_name in version_names) {
    reprodlog_list[[iter]] <- ReadLog(test_name, version_name, reprodlog_name)
    iter <- iter + 1
  }
}
# Transform into tidy data
reprodlog_df <- bind_rows(reprodlog_list) %>% 
  dplyr::select(-X7) %>% 
  pivot_longer(cols = -c("Time", "SpeciesName", "testname", "necnver"))
# Plot
plt_test01_reprod <- reprodlog_df %>% 
  filter(testname == 'test01_SingleCohort') %>% 
  ggplot(aes(x = Time, y = value, group = interaction(testname, necnver, name, SpeciesName), color = necnver)) +
  geom_line() + facet_grid(name~SpeciesName, scales = 'free') + 
  labs(title = 'NECN-reproduction-log.csv of test01_SingleCohort', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test02_reprod <- reprodlog_df %>% 
  filter(testname == 'test02_TwoCohorts_SameSpp') %>% 
  ggplot(aes(x = Time, y = value, group = interaction(testname, necnver, name, SpeciesName), color = necnver)) +
  geom_line() + facet_grid(name~SpeciesName, scales = 'free') + 
  labs(title = 'NECN-reproduction-log.csv of test02_TwoCohorts_SameSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test03_reprod <- reprodlog_df %>% 
  filter(testname == 'test03_TwoCohorts_DifferentSpp') %>% 
  ggplot(aes(x = Time, y = value, group = interaction(testname, necnver, name, SpeciesName), color = necnver)) +
  geom_line() + facet_grid(name~SpeciesName, scales = 'free') + 
  labs(title = 'NECN-reproduction-log.csv of test03_TwoCohorts_DifferentSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_reprod <- plt_test01_reprod / plt_test02_reprod / plt_test03_reprod
ggsave('fig04_NECN-reproduction-log.pdf', plt_reprod, width = 27, height = 27)

# Compare numeric values
reprodlog_diff_df <- reprodlog_df %>% 
  pivot_wider(names_from = 'necnver', values_from = 'value') %>% 
  mutate(difference = grasses67 - original67)
summary(reprodlog_diff_df$difference)
#    Min. 1st Qu.  Median    Mean 3rd Qu.    Max. 
#       0       0       0       0       0       0


## 5. NECN-calibrate-log.csv ----
# Read data
callog_list <- list(); iter <- 1
for (test_name in test_names) {
  for (version_name in version_names) {
    callog_list[[iter]] <- ReadLog(test_name, version_name, callog_name)
    iter <- iter + 1
  }
}
# Transform into tidy data
callog_df <- bind_rows(callog_list) %>% 
  dplyr::select(-starts_with('X')) %>% 
  pivot_longer(cols = -c("Year", "Month", "ClimateRegionIndex", "SpeciesName", "CohortAge", "testname", "necnver")) %>% 
  mutate(date = lubridate::ymd(paste(Year + 2015, Month, 1, sep = '-')),
         cohort.id = CohortAge - Year) %>% 
  mutate(cohort.id = if_else(Month == 6, cohort.id - 1, cohort.id))
# Plot
plt_test01_cal <- callog_df %>% 
  filter(testname == 'test01_SingleCohort') %>% 
  ggplot(aes(x = date, y = value, group = interaction(testname, necnver, name, SpeciesName, cohort.id), color = necnver)) +
  geom_line() + facet_wrap(~name, scales = 'free') + 
  labs(title = 'NECN-calibrate-log.csv of test01_SingleCohort', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test02_cal <- callog_df %>% 
  filter(testname == 'test02_TwoCohorts_SameSpp') %>% 
  ggplot(aes(x = date, y = value, group = interaction(testname, necnver, name, SpeciesName, cohort.id), color = necnver)) +
  geom_line() + facet_wrap(~name, scales = 'free') + 
  labs(title = 'NECN-calibrate-log.csv of test02_TwoCohorts_SameSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_test03_cal <- callog_df %>% 
  filter(testname == 'test03_TwoCohorts_DifferentSpp') %>% 
  ggplot(aes(x = date, y = value, group = interaction(testname, necnver, name, SpeciesName, cohort.id), color = necnver)) +
  geom_line() + facet_wrap(~name, scales = 'free') + 
  labs(title = 'NECN-calibrate-log.csv of test03_TwoCohorts_DifferentSpp', 
       subtitle = 'Grasses v67 vs. Original v67') + scale_color_calc() + theme_few()
plt_cal <- plt_test01_cal / plt_test02_cal / plt_test03_cal
ggsave('fig05_NECN-calibrate-log.pdf', plt_cal, width = 27, height = 27)

# Compare numeric values
callog_diff_df <- callog_df %>% 
  pivot_wider(names_from = 'necnver', values_from = 'value') %>% 
  mutate(difference = grasses67 - original67)
summary(callog_diff_df$difference)
#   Min. 1st Qu.  Median    Mean 3rd Qu.    Max.    NA's 
#      0       0       0       0       0       0  512880
# "NA's" == 4 new output variables. these can be ignored.



