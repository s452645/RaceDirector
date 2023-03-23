import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CarsComponent } from './components/containers/cars/cars.component';
import { HomeComponent } from './components/containers/home/home.component';
import { OwnersComponent } from './components/containers/owners/owners.component';
import SeasonsComponent from './components/containers/seasons/seasons.component';
import { SettingsComponent } from './components/containers/settings/settings.component';
import { SeasonDetailsComponent } from './components/seasons/season-details/season-details.component';
import { RouteTitle } from './services/route-title.service';

const routes: Routes = [
  { path: 'home', component: HomeComponent, title: RouteTitle.HOME },
  { path: 'seasons', component: SeasonsComponent, title: RouteTitle.SEASONS },
  {
    path: 'seasons/:id',
    component: SeasonDetailsComponent,
    title: RouteTitle.SEASONS,
  },
  { path: 'cars', component: CarsComponent, title: RouteTitle.CARS },
  { path: 'owners', component: OwnersComponent, title: RouteTitle.OWNERS },
  {
    path: 'settings',
    component: SettingsComponent,
    title: RouteTitle.SETTINGS,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
