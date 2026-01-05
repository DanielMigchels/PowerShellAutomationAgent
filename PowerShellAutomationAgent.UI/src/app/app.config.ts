import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideIcons } from '@ng-icons/core';
import { heroArrowDownTray, heroCheckCircle, heroCodeBracket, heroPencil, heroRocketLaunch, heroXCircle, heroChevronLeft, heroPlay, heroEye, heroClock } from '@ng-icons/heroicons/outline';
import { authInterceptor } from './interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideIcons({heroArrowDownTray, heroCheckCircle, heroXCircle, heroCodeBracket, heroPencil, heroRocketLaunch, heroChevronLeft, heroPlay, heroEye, heroClock}),
  ]
};
