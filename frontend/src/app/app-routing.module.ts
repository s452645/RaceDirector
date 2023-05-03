import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CarsComponent } from './components/containers/cars/cars.component';
import { HomeComponent } from './components/containers/home/home.component';
import { OwnersComponent } from './components/containers/owners/owners.component';
import SeasonsComponent from './components/containers/seasons/seasons.component';
import { SettingsComponent } from './components/containers/settings/settings.component';
import { SeasonDetailsComponent } from './components/seasons/season-details/season-details.component';
import { SeasonEventComponent } from './components/seasons/season-event/season-event.component';
import { RouteTitle } from './services/route-title.service';
import { SeasonEventRoundComponent } from './components/seasons/season-event-round/season-event-round/season-event-round.component';
import { SeasonEventRoundRaceComponent } from './components/seasons/season-event-round-race/season-event-round-race.component';
import { RaceHeatViewComponent } from './components/seasons/season-event-round-race/race-heat-view-temp/race-heat-view.component';

const routes: Routes = [
  { path: 'home', component: HomeComponent, title: RouteTitle.HOME },
  {
    path: 'seasons',
    component: SeasonsComponent,
    title: RouteTitle.SEASONS,
  },
  {
    path: 'seasons/:id',
    component: SeasonDetailsComponent,
  },
  {
    path: 'seasons/:seasonId/events/:eventId',
    component: SeasonEventComponent,
  },
  {
    path: 'seasons/:seasonId/events/:eventId/rounds/:roundId',
    component: SeasonEventRoundComponent,
  },
  {
    path: 'seasons/:seasonId/events/:eventId/rounds/:roundId/races/:raceId',
    component: SeasonEventRoundRaceComponent,
  },
  {
    path: 'seasons/:seasonId/events/:eventId/rounds/:roundId/races/:raceId/heats/:heatId',
    component: RaceHeatViewComponent,
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
