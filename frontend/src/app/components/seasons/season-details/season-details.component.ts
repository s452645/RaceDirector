import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-season-details',
  templateUrl: './season-details.component.html',
  styleUrls: ['./season-details.component.css'],
})
export class SeasonDetailsComponent implements OnInit {
  public seasonId: string | null = null;

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.seasonId = this.route.snapshot.paramMap.get('id');
  }
}
