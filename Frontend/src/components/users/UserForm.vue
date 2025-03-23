<!-- src/components/UserForm.vue -->
<template>
  <div>
    <h2>Add New User</h2>
    <form @submit.prevent="submitForm">
      <div>
        <label>Name:</label>
        <input v-model="newUser.name" required />
      </div>
      <div>
        <label>Username:</label>
        <input v-model="newUser.username" required />
      </div>
      <div>
        <label>Email:</label>
        <input v-model="newUser.email" type="email" required />
      </div>
      <!-- Add additional fields as needed -->
      <button type="submit">Add User</button>
    </form>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { useUserStore } from '../../store/userStore';

interface NewUser {
  name: string;
  username: string;
  email: string;
  // Optionally add address, phone, etc.
}

const newUser = ref<NewUser>({
  name: '',
  username: '',
  email: ''
});

const userStore = useUserStore();

const submitForm = async () => {
  await userStore.addUser(newUser.value);
  newUser.value = { name: '', username: '', email: '' }; // Reset the form after submission
};
</script>
