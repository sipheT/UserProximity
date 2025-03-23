<!-- src/components/UserList.vue -->
<template>
  <div>
    <h2>User List</h2>
    <div v-if="userStore.loading">Loading...</div>
    <div v-if="userStore.error">{{ userStore.error }}</div>
    <ul v-else>
      <li v-for="user in userStore.users" :key="user.id">
        <strong>{{ user.name }}</strong> ({{ user.email }})
      </li>
    </ul>
  </div>
</template>

<script lang="ts" setup>
import { onMounted } from 'vue';
import { useUserStore } from '../../store/userStore';

const userStore = useUserStore();

onMounted(() => {
  userStore.fetchUsers();
});
</script>
