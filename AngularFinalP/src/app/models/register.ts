export interface Register {
  userName: string;
  password: string;
  comparePassword?: string; // for client-side validation
  fullName?: string | null;
  email?: string | null;
}
