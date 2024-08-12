<template>
    <v-container>
      <v-btn @click="openDialog('add')">Add Salary</v-btn>

      <v-data-table
        :headers="headers"
        :items="salaries"
        :items-per-page="itemsPerPage"
        :page.sync="page"
        :server-items-length="totalItems"
        :loading="loading"
        @update:options="fetchSalaries"
      >

      <template v-slot:item="{ item }">
          <tr>
            <td>{{ item.company }}</td>
            <td>{{ item.role }}</td>
            <td>{{ item.yearsOfExperience }}</td>
            <td>{{ formatCurrency(item.salaries) }}</td>
            <td>{{ formatCurrency(item.yearlyBonus) }}</td>
            <td>
              <v-btn @click="openDialog('edit', item)">Edit</v-btn>
              <v-btn @color="error" @click="openDeleteDialog(item)">Delete</v-btn>
            </td>
          </tr>
      </template>

      <template v-slot:top>
        <v-pagination
          v-model:page="page"
          :length="Math.ceil(totalItems / itemsPerPage)"
        ></v-pagination>
      </template>

      <template v-slot:header="{ props: { headers } }">
          <thead>
              <tr>
                  <th v-for="header in headers" :key="header.key">
                      {{ header.title }}
                  </th>
              </tr>
              <tr>
                  <th v-for="header in headers" :key="header.key">
                      <v-text-field
                          v-model="filters[headers.key]"
                          :placeholder="`Filter ${header.title}`"
                          dense
                          hide-details
                          @input="applyFilter"></v-text-field>
                  </th>
              </tr>
          </thead>
      </template>
  
      </v-data-table>

      <v-dialog v-model="dialogVisible" max-width="600px">
        <v-card>
          <v-card-title>
            <span class="text-h5">{{ dialogTitle }}</span>
          </v-card-title>

          <v-card-text>
            <v-form>
              <v-text-field v-model="form.company" label="Company" required></v-text-field>
              <v-text-field v-model="form.role" label="Role" required></v-text-field>
              <v-text-field v-model="form.yearsOfExperience" label="Years of Experience" required type="number"></v-text-field>
              <v-text-field v-model="form.salaries" label="Salary" required type="number"></v-text-field>
              <v-text-field v-model="form.yearlyBonus" label="Yearly Bonus" required type="number"></v-text-field>
            </v-form>
          </v-card-text>

          <v-card-actions>
            <v-btn color="primary" @click="saveSalary">Save</v-btn>
            <v-btn text @click="closeDialog">Cancel</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

      <v-dialog v-model="deleteDialog" max-width="500">
        <v-card>
          <v-card-title>Confirm Delete</v-card-title>
          <v-card-text>
            Are you sure you want to delete this salary record for {{ selectedItem?.company }}?
          </v-card-text>
          <v-card-actions>
            <v-btn color="primary" text @click="deleteDialog = false">Cancel</v-btn>
            <v-btn color="error" @click="confirmDelete">Delete</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-container>
  </template>
  
  <script>
  import axios from 'axios';
  
  export default {
    data() {
      return {
        headers: [
          { title: 'Company', key: 'company' },
          { title: 'Role', key: 'role' },
          { title: 'Years of Experience', key: 'yearsOfExperience' },
          { title: 'Salary', key: 'salaries' },
          { title: 'Yearly Bonus', key: 'yearlyBonus' },
        ],
        salaries: [],
        page: 1,
        itemsPerPage: 20,
        totalItems: 0,
        loading: false,
        dialogVisible: false,
        dialogTitle: '',
        form: {
          company: '',
          role: '',
          yearsOfExperience: '',
          salaries: '',
          yearlyBonus: '',
        },
        action: '',
        deleteDialog: false,
        selectedItem: null,
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
      openDialog(action, item=null) {
        this.action = action;
        this.dialogTitle = action === 'add' ? 'Add Salary' : 'Edit Salary';

        if (action === 'edit' && item){
          this.form = { ...item };
        } else {
          this.form = {
            id: null,
            company: '',
            role: '',
            yearsOfExperience: '',
            salaries: '',
            yearlyBonus: '',
          };
        }
        this.dialogVisible = true;
      },
      closeDialog() {
        this.dialogVisible = false;
      },
      async saveSalary() {
        try {
          const formattedSalaries = this.form.salaries.replace(/[^0-9]/g, '');
          const formattedYearlyBonus = this.form.yearlyBonus ? this.form.yearlyBonus.replace(/[^0-9]/g, '') : '';

          const dataToSend = {
            ...this.form,
            salaries: parseFloat(formattedSalaries),
            yearlyBonus: parseFloat(formattedYearlyBonus),
          };

          if (this.action === 'add'){
            delete dataToSend.id;
          }

          if (this.action === 'add') {
            await axios.post('https://localhost:7188/api/salaries', dataToSend);
          } else if (this.action === 'edit') {
            await axios.put(`https://localhost:7188/api/salaries/${this.form.id}`, dataToSend);
          }

          this.fetchSalaries();
          this.closeDialog();
        } catch (error) {
          console.error('Error saving salary:', error);
        }
      },
      async deleteSalary(id) {
        try{
          await axios.delete(`https://localhost:7188/api/salaries/${id}`);
          this.fetchSalaries();
        } catch (error) {
          console.error('Error deleting salary:', error.response ? error.response.data : error.message);
        }
      },
      async confirmDelete() {
        try {
          await this.deleteSalary(this.selectedItem.id);
          this.deleteDialog = false; 
        } catch (error) {
          console.error('Error deleting salary:', error.response ? error.response.data : error.message);
        }
      },
      openDeleteDialog(item) {
        this.selectedItem = item;
        this.deleteDialog = true;
      },
      formatCurrency(value) {
        if (value === null || value === undefined) return '';
        return `IDR ${Number(value).toLocaleString('id-ID')}`;
      },
    },    
    created() {
      this.fetchSalaries();
    },
  };
  </script>
  