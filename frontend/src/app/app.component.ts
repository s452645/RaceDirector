import { PrimeNGConfig } from 'primeng/api';
import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { invoke } from '@tauri-apps/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'race-director-frontend';

  constructor(
    private primengConfig: PrimeNGConfig,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    this.primengConfig.ripple = true;
    this.translateService.setDefaultLang('en');

    // invoking rust function ('command')
    invoke('greet', { name: 'Bartek' }).then(response => console.log(response));
  }

  translate(lang: string) {
    this.translateService.use(lang);
    this.translateService
      .get('primeng')
      .subscribe(res => this.primengConfig.setTranslation(res));
  }
}
