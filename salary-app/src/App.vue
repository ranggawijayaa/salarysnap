\<template>
  <v-app>
    <v-main>
      <v-container>
        <h1>Salary Data</h1>
        <SalaryTable />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup>
import SalaryTable from './components/SalaryTable.vue';
</script>