import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { EditUserDialogComponent } from '../edit-user-dialog/edit-user-dialog.component';
import { UserDomain } from 'src/app/models/userDomain.model';
import { RegistrationComponent } from '../registration/registration.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  users: UserDomain[] = [];
  displayedColumns: string[] = ['username', 'password', 'sensorIds', 'actions'];

  constructor(private http: HttpClient, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.http.get<UserDomain[]>('https://localhost:7246/api/admin/users').subscribe(data => {
      this.users = data;
    });
  }

  deleteUser(userId: string): void {
    this.http.delete(`https://localhost:7246/api/admin/delete/${userId}`).subscribe(() => {
      this.loadUsers();
    });
  }

  editUser(user: UserDomain): void {
    const dialogRef = this.dialog.open(EditUserDialogComponent, {
      width: '500px',
      data: user
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  addUser(): void {
    const dialogRef = this.dialog.open(RegistrationComponent, {
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }
}