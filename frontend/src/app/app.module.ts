import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { ButtonModule } from 'primeng/button';

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
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ButtonModule,
    HttpClientModule,
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
