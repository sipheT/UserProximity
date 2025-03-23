// src/store/userStore.ts
import { defineStore } from 'pinia';
import axios from 'axios';

export interface User {
  id: number;
  name: string;
  username: string;
  email: string;
  address: {
    street: string;
    suite: string;
    city: string;
    zipcode: string;
    geo: {
      lat: string;
      lng: string;
    }
  };
  phone: string;
  website: string;
  company: {
    name: string;
    catchPhrase: string;
    bs: string;
  };
}

export const useUserStore = defineStore('user', {
  state: () => ({
    users: [] as User[],
    loading: false,
    error: ''
  }),
  actions: {
    async fetchUsers() {
      this.loading = true;
      this.error = '';
      try {
        // Adjust the URL if your backend is running on a different port
        const response = await axios.get<User[]>('https://localhost:7066/api/users');
        this.users = response.data;
      } catch (err) {
        this.error = 'Failed to load users';
        console.error(err);
      } finally {
        this.loading = false;
      }
    },
    async addUser(user: Omit<User, 'id'>) {
      try {
        const response = await axios.post<User>('https://localhost:7066/api/users', user);
        this.users.push(response.data);
      } catch (err) {
        this.error = 'Failed to add user';
        console.error(err);
      }
    }
    // Additional update and delete actions can be added similarly.
  }
});
