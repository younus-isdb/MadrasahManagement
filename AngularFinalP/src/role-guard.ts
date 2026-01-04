//import { CanActivateFn, inject } from '@angular/core';
//import { Router } from '@angular/router';
//import { AuthService } from './app/service/authservice';

//export const RoleGuard: CanActivateFn = (route, state) => {
//  const authService = inject(AuthService);
//  const router = inject(Router);

//  const expectedRoles = route.data['roles'] as string[];
//  const user = authService.getUser();

//  if (!authService.isLoggedIn() || !expectedRoles.some(r => user?.roles.includes(r))) {
//    router.navigate(['/login']);
//    return false;
//  }

//  return true;
//};
