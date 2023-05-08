import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { RadioButtonModule } from 'primeng/radiobutton';
import { RippleModule } from 'primeng/ripple';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { CalendarModule } from 'primeng/calendar';
import { CardModule } from 'primeng/card';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { InputNumberModule } from 'primeng/inputnumber';
import { ChipsModule } from 'primeng/chips';
import { SelectButtonModule } from 'primeng/selectbutton';
import { MultiSelectModule } from 'primeng/multiselect';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SidebarComponent } from './components/layout/sidebar/sidebar.component';
import { MainContainerComponent } from './components/layout/main-container/main-container.component';
import { SidebarIconComponent } from './components/layout/sidebar/sidebar-icon/sidebar-icon.component';
import { TitleBarComponent } from './components/layout/title-bar/title-bar.component';
import { HomeComponent } from './components/containers/home/home.component';
import { CarsComponent } from './components/containers/cars/cars.component';
import { OwnersComponent } from './components/containers/owners/owners.component';
import { BaseContainerComponent } from './components/containers/base-container/base-container.component';
import SeasonsComponent from './components/containers/seasons/seasons.component';
import { SettingsComponent } from './components/containers/settings/settings.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PicosSyncStatusComponent } from './components/layout/sidebar/picos-sync-status/picos-sync-status.component';
import { SeasonDetailsComponent } from './components/seasons/season-details/season-details.component';
import { SeasonEventComponent } from './components/seasons/season-event/season-event.component';
import { NewSeasonEventFormComponent } from './components/seasons/season-event/new-season-event-form/new-season-event-form.component';
import { CircuitFormComponent } from './components/circuit/circuit-form/circuit-form.component';
import { SeasonEventScoreRulesFormComponent } from './components/seasons/season-event/season-event-score-rules-form/season-event-score-rules-form.component';
import { RoundFormComponent } from './components/seasons/season-event-round/round-form/round-form.component';
import { RaceFormComponent } from './components/seasons/season-event-round/round-form/race-form/race-form.component';
import { BoardSensorsDetailsComponent } from './components/containers/settings/board-sensors-details/board-sensors-details.component';
import { SeasonEventRoundComponent } from './components/seasons/season-event-round/season-event-round/season-event-round.component';
import { SeasonEventRoundRaceComponent } from './components/seasons/season-event-round-race/season-event-round-race.component';
import { RaceHeatViewComponent } from './components/seasons/season-event-round-race/race-heat-view/race-heat-view.component';
import { RoundRaceViewComponent } from './components/seasons/season-event-round/season-event-round/round-race-view/round-race-view.component';
import { SeasonEventRoundRaceHeatComponent } from './components/seasons/season-event-round-race-heat/season-event-round-race-heat.component';
import { HeatCurrentRaceTableComponent } from './components/seasons/season-event-round-race-heat/heat-current-race-table/heat-current-race-table.component';
import { HeatPointsTableComponent } from './components/seasons/season-event-round-race-heat/heat-points-table/heat-points-table.component';
import { HeatRaceSectorTableComponent } from './components/seasons/season-event-round-race-heat/heat-race-sector-table/heat-race-sector-table.component';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [
    AppComponent,
    SidebarComponent,
    MainContainerComponent,
    SidebarIconComponent,
    TitleBarComponent,
    HomeComponent,
    SeasonsComponent,
    CarsComponent,
    OwnersComponent,
    SettingsComponent,
    BaseContainerComponent,
    PicosSyncStatusComponent,
    SeasonDetailsComponent,
    SeasonEventComponent,
    NewSeasonEventFormComponent,
    CircuitFormComponent,
    SeasonEventScoreRulesFormComponent,
    RoundFormComponent,
    RaceFormComponent,
    BoardSensorsDetailsComponent,
    SeasonEventRoundComponent,
    SeasonEventRoundRaceComponent,
    RaceHeatViewComponent,
    RoundRaceViewComponent,
    SeasonEventRoundRaceHeatComponent,
    HeatCurrentRaceTableComponent,
    HeatPointsTableComponent,
    HeatRaceSectorTableComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    InputTextModule,
    CheckboxModule,
    CalendarModule,
    DialogModule,
    ButtonModule,
    DropdownModule,
    ConfirmPopupModule,
    RadioButtonModule,
    RippleModule,
    FormsModule,
    ToastModule,
    TableModule,
    AppRoutingModule,
    ButtonModule,
    InputNumberModule,
    HttpClientModule,
    ReactiveFormsModule,
    ChipsModule,
    SelectButtonModule,
    CardModule,
    MultiSelectModule,
    TranslateModule.forRoot({
      defaultLanguage: 'en',
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
    }),
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
