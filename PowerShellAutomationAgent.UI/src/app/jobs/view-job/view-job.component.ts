import { AfterViewInit, Component, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { JobService } from '../../../services/jobs/job.service';
import { JobResponseModel } from '../../../services/jobs/models/job-response-model';
import { NgIconComponent } from '@ng-icons/core';
import * as monaco from 'monaco-editor';
import loader from '@monaco-editor/loader';

@Component({
  selector: 'app-view-job',
  imports: [NgIconComponent],
  templateUrl: './view-job.component.html',
  styleUrl: './view-job.component.css'
})
export class ViewJobComponent implements AfterViewInit {

  jobId: string = '';
  job: JobResponseModel | undefined;

  private editor: monaco.editor.IStandaloneCodeEditor | undefined;

  constructor(private route: ActivatedRoute, private jobService: JobService, private host: ElementRef, private router: Router) { }
  
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.jobId = params.get('id') ?? '';
    });
  }

  private static monacoInitialized = false;

  async ngAfterViewInit() {
    const monaco = await loader.init();

    this.editor = monaco.editor.create(
      this.host.nativeElement.querySelector('.editor-container'),
      {
        value: '',
        language: 'powershell',
        theme: 'vs-dark',
        fontSize: 14,
        minimap: { enabled: true },
        wordWrap: 'on',
        automaticLayout: true,
        scrollBeyondLastLine: false
      }
    );

    this.getJob();
  }

  back() {
    this.router.navigate(['/jobs']);
  }

  getValue() {
    return this.editor?.getValue() ?? '';
  }

  setValue(value: string) {
    this.editor?.setValue(value);
  }

  getJob(): void {
    this.jobService.getJob(this.jobId).subscribe({
      next: (job) => {
        this.job = job;
        this.setValue(job.output ?? "");
      },
    });
  }
}
