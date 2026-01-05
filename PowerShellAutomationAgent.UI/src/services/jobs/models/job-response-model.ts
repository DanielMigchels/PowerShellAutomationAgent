export interface JobResponseModel {
  id: number;
  state: JobStates;
  script: string | undefined;
  output: string | undefined;
  hasArtifacts: boolean;
  projectName: string;
  createdOnUtc: Date;
  startedOnUtc: Date;
  finishedOnUtc: Date;
}

export enum JobStates {
  Queued = 1,
  InProgress = 2,
  Succeeded = 3,
  Failed = 4,
}