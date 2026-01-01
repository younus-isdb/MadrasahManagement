import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './app/service/authservice';

export const AppGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService) as AuthService; // type assertion
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  return true;
};
