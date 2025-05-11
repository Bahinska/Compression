import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { UserDomain } from 'src/app/models/userDomain.model';

@Component({
  selector: 'app-edit-user-dialog',
  templateUrl: './edit-user-dialog.component.html',
  styleUrls: ['./edit-user-dialog.component.css']
})
export class EditUserDialogComponent {
  editForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    public dialogRef: MatDialogRef<EditUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UserDomain) {

    this.editForm = this.fb.group({
      password: [data.password, Validators.required],
      username: [data.username, [Validators.required, Validators.email]],
      sensorIds: this.fb.array(data.sensorIds ? data.sensorIds.map(sensorId => this.fb.control(sensorId, Validators.required)) : [])
    });
  }
  get sensors(): FormArray {
    return this.editForm.get('sensorIds') as FormArray;
  }

  addSensor(): void {
    this.sensors.push(this.fb.control('', Validators.required));
  }

  removeSensor(index: number): void {
    this.sensors.removeAt(index);
  }

  onSave(): void {
    const updatedUser: UserDomain = {
      ...this.data,
      ...this.editForm.value
    };

    console.log(updatedUser.id);
    console.log(updatedUser);

    this.http.put(`https://localhost:7246/api/admin/update/${updatedUser.id}`, updatedUser).subscribe(() => {
      this.dialogRef.close(true);
    });
  }
}