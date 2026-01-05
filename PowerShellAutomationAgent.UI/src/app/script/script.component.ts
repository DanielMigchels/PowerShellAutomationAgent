import { AfterViewInit, Component, ElementRef, OnDestroy } from '@angular/core';
import * as monaco from 'monaco-editor';
import loader from '@monaco-editor/loader';
import { NgIcon } from "@ng-icons/core";
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ProjectService } from '../../services/projects/project.service';
import { EditProjectRequestModel } from '../../services/projects/models/edit-project-request-model';

@Component({
  selector: 'app-script',
  imports: [NgIcon],
  templateUrl: './script.component.html',
  styleUrl: './script.component.css'
})
export class ScriptComponent implements AfterViewInit, OnDestroy {

  private editor: monaco.editor.IStandaloneCodeEditor | undefined;

  formGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    script: new FormControl(''),
  });

  projectId: string = "";

  constructor(private route: ActivatedRoute, private projectService: ProjectService, private host: ElementRef, private router: Router) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.projectId = params.get('id') ?? '';
    });
  }

  getProject() {
    this.projectService.getProject(this.projectId).subscribe({
      next: project => {
        this.formGroup.patchValue(project);
        this.setValue(project.script);
      },
      error: () => {
        this.router.navigate([`/projects`]);
      }
    });
  }

  private static monacoInitialized = false;

  async ngAfterViewInit() {
    const monaco = await loader.init();

    if (!ScriptComponent.monacoInitialized) {
      this.registerPowerShellLanguage(monaco);
      ScriptComponent.monacoInitialized = true;
    }

    this.editor = monaco.editor.create(
      this.host.nativeElement.querySelector('.editor-container'),
      {
        value: '',
        language: 'powershell',
        theme: 'vs-dark',
        fontSize: 14,
        minimap: { enabled: false },
        wordWrap: 'on',
        automaticLayout: true,
        scrollBeyondLastLine: false
      }
    );

    this.getProject();
  }

  private registerPowerShellLanguage(monaco: any) {
    monaco.languages.register({ id: 'powershell' });

    monaco.languages.setMonarchTokensProvider('powershell', {
      tokenizer: {
        root: [
          [/#.*/, "comment"],
          [/\$[A-Za-z_]\w*/, "variable"],
          [
            // PowerShell language keywords
            /\b(function|param|if|elseif|else|foreach|while|for|do|until|switch|break|continue|return|try|catch|finally|throw|trap|begin|process|end|workflow)\b/i,
            "keyword"
          ],
          [
            // Cmdlets + common commands + CLI tools
            /\b(Write-Host|Write-Output|Write-Error|Get-ChildItem|Get-Item|Set-Item|Remove-Item|Copy-Item|Move-Item|New-Item|Test-Path|Start-Process|Stop-Process|Get-Process|Get-Service|Start-Service|Stop-Service|Restart-Service|Get-Content|Set-Content|Add-Content|Clear-Content|Get-Command|Get-Help|Import-Module|Export-ModuleMember|Invoke-Command|Invoke-Expression|Start-Job|Wait-Job|Receive-Job|Remove-Job|Out-File|Out-String|dotnet|git|mkdir|rmdir|del|copy|move|ren|cls|echo|clear|ls|pwd|cd)\b/i,
            "keyword"
          ],
          [/".*?"/, "string"],
          [/\'.*?\'/, "string"]
        ]
      }
    });

    monaco.languages.registerCompletionItemProvider('powershell', {
      provideCompletionItems: (model: any, position: any) => {
        const wordInfo = model.getWordUntilPosition(position);
        const wordRange = new monaco.Range(
          position.lineNumber,
          wordInfo.startColumn,
          position.lineNumber,
          wordInfo.endColumn
        );

        const keywords = [
          // PowerShell language keywords
          "function", "param", "if", "elseif", "else", "foreach", "while", "for",
          "do", "until", "switch", "break", "continue", "return", "try", "catch",
          "finally", "throw", "trap", "begin", "process", "end", "workflow",

          // PowerShell cmdlets
          "Write-Host", "Write-Output", "Write-Error", "Get-ChildItem", "Get-Item",
          "Set-Item", "Remove-Item", "Copy-Item", "Move-Item", "New-Item",
          "Test-Path", "Start-Process", "Stop-Process", "Get-Process", "Get-Service",
          "Start-Service", "Stop-Service", "Restart-Service", "Get-Content",
          "Set-Content", "Add-Content", "Clear-Content", "Get-Command", "Get-Help",
          "Import-Module", "Export-ModuleMember", "Invoke-Command", "Invoke-Expression",
          "Start-Job", "Wait-Job", "Receive-Job", "Remove-Job", "Out-File", "Out-String",

          // dotnet commands
          "dotnet", "dotnet build", "dotnet run", "dotnet test",
          "dotnet publish", "dotnet restore", "dotnet clean",

          // git commands
          "git", "git clone", "git commit", "git push", "git pull",
          "git status", "git checkout", "git merge", "git branch", "git init",

          // Common CLI commands often used in PowerShell
          "mkdir", "rmdir", "del", "copy", "move", "ren", "cls", "clear", "ls", "pwd", "cd", "echo"
        ];

        const suggestions = keywords.map(keyword => ({
          label: keyword,
          kind: monaco.languages.CompletionItemKind.Keyword,
          insertText: keyword,
          range: wordRange
        }));

        suggestions.push({
          label: "$Variable",
          kind: monaco.languages.CompletionItemKind.Variable,
          insertText: "$Variable",
          range: wordRange
        });

        return { suggestions };
      }
    });
  }

  ngOnDestroy() {
    this.editor?.dispose();
  }

  getValue() {
    return this.editor?.getValue() ?? '';
  }

  setValue(value: string) {
    this.editor?.setValue(value);
  }

  back() {
    this.router.navigate(['/projects']);
  }

  save() {
    this.updateProject();
  }

  updateProject() {
    this.formGroup.patchValue({ script: this.getValue() });

    if (!this.formGroup.valid) {
      this.formGroup.markAllAsTouched();
      return;
    }

    this.projectService.editProject(this.projectId, this.formGroup.value as EditProjectRequestModel).subscribe({
      next: x => {
        this.formGroup.reset();
        this.router.navigate([`/projects`]);
      },
      error: () => {

      }
    });
  }
}
