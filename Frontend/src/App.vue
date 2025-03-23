<!-- src/App.vue -->
<template>
  <div id="app">
    <h1>UserProximity Frontend</h1>
    <UserForm />
    <UserList />
  </div>
</template>

<script lang="ts" setup>
import UserForm from './components/users/UserForm.vue';
import UserList from './components/users/UserList.vue';
</script>

<style>
/* Add basic styling as needed */
body {
  font-family: Arial, sans-serif;
}
#app {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}
</style>
