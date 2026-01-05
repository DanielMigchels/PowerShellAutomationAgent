import { JobResponseModel } from "../../jobs/models/job-response-model";

export interface DashboardResponseModel {
  failedJobsCount: number;
  runningJobsCount: number;
  successfulJobsCount: number;
  recentJobs: JobResponseModel[];
}
