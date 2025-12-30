import { Routes } from '@angular/router';
import { TicketDetailComponent } from './pages/ticket-detail/ticket-detail';
import { HomeComponent } from './pages/home/home';
import { LoginComponent } from './pages/login/login';
export const routes: Routes = [
  // 2. Uygulama açılınca (adres boşsa) direkt 'login'e yönlendir
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // 3. Login sayfasının adresi
  { path: 'login', component: LoginComponent },

  // 4. Ana Sayfa artık '/home' adresinde çalışacak
  { path: 'home', component: HomeComponent },
  
  { path: 'ticket/:id', component: TicketDetailComponent },
  
  // Hatalı/Bilinmeyen adres girilirse login'e at
  { path: '**', redirectTo: 'login' }
];