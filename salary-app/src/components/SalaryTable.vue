<template>
    <v-container>
      <v-data-table
        :headers="headers"
        :items="salaries"
        :items-per-page="itemsPerPage"
        :page.sync="page"
        :server-items-length="totalItems"
        :loading="loading"
        @update:options="(options) => fetchSalaries(options)"
      >
        <template v-slot:top>
          <v-pagination
            v-model:page="page"
            :length="Math.ceil(totalItems / itemsPerPage)"
          ></v-pagination>
        </template>
  
        <!-- Ensure the item slot is used correctly -->
        <template v-slot:item="props">
          <tr>
            <td>{{ props.item.company }}</td>
            <td>{{ props.item.role }}</td>
            <td>{{ props.item.yearsOfExperience }}</td>
            <td>{{ props.item.salaries }}</td>
            <td>{{ props.item.yearlyBonus }}</td>
          </tr>
        </template>
      </v-data-table>
    </v-container>
  </template>
  
  <script>
  import axios from 'axios';
  
  export default {
    data() {
      return {
        headers: [
          { title: 'Company', value: 'company', key: 'company' },
          { title: 'Role', value: 'role', key: 'company' },
          { title: 'Years of Experience', value: 'yearsOfExperience', key: 'yearsOfExperience' },
          { title: 'Salary', value: 'salaries', key: 'salaries' },
          { title: 'Yearly Bonus', value: 'yearlyBonus', key: 'yearlyBonus' },
        ],
        salaries: [],
        page: 1,
        itemsPerPage: 20,
        totalItems: 0,
        loading: false,
      };
    },
    methods: {
      async fetchSalaries(options = { sortBy: [], sortDesc: [] }) {
        this.loading = true;
        try {
          const sortBy = Array.isArray(options.sortBy) && options.sortBy.length ? options.sortBy[0] : '';
          const sortDesc = Array.isArray(options.sortDesc) && options.sortDesc.length ? options.sortDesc[0] : false;
  
          const response = await axios.get('https://localhost:7188/api/salaries', {
            params: {
              page: this.page,
              per_page: this.itemsPerPage,
              sortBy: sortBy,
              sortDesc: sortDesc,
            },
          });
  
          this.salaries = response.data.items || [];
          this.totalItems = response.data.totalItems || 0;
        } catch (error) {
          console.error('Error fetching salaries:', error);
        } finally {
          this.loading = false;
        }
      },
    },
    created() {
      this.fetchSalaries({ sortBy: [], sortDesc: [] });
    },
  };
  </script>
  