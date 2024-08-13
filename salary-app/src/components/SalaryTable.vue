<template>
    <v-card
      title="Salary Data"
      flat
    >
      <v-btn @click="openDialog('add')">Add Salary</v-btn>
      <v-btn 
        @click="copySelectedRow"
        :disabled="selectedRows.length !== 1"
        class="ml-2"
        >
        Copy Selected
      </v-btn>

      <template v-slot:text>
        <v-text-field
          v-model="search"
          label="Search"
          prepend-inner-icon="mdi-magnify"
          variant="outlined"
          hide-details
          single-line
        ></v-text-field>
      </template>

      <v-data-table
        v-model:selected="selectedRows"
        :headers="headers"
        :items="salaries"
        :loading="loading"
        @update:options="fetchSalaries"
        show-select
        item-value="id"
        :search="search"
      >
        <template v-slot:item="{ item }">
              <tr>
                <td>
                  <v-checkbox
                    v-model="selectedRows"
                    :value="item"
                    hide-details
                  ></v-checkbox>
                </td>
                <td>{{ item.company }}</td>
                <td>{{ item.role }}</td>
                <td>{{ item.yearsOfExperience }}</td>
                <td>{{ formatCurrency(item.salaries) }}</td>
                <td>{{ formatCurrency(item.yearlyBonus) }}</td>
                <td align="center">
                  <v-btn @click="openDialog('edit', item)">Edit</v-btn>
                  <v-btn @color="error" @click="openDeleteDialog(item)">Delete</v-btn>
                </td>
              </tr>
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
              <v-text-field v-model.number="form.yearsOfExperience" label="Years of Experience" required type="number"></v-text-field>
              <v-text-field v-model.number="form.salaries" label="Salary" required type="number"></v-text-field>
              <v-text-field v-model.number="form.yearlyBonus" label="Yearly Bonus" required type="number"></v-text-field>
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
      
      <v-snackbar
        v-model="snackbar"
        :color="snackbarColor"
        :timeout="3000"
      >
        {{ snackbarText }}
      </v-snackbar>
    </v-card>
</template>
  
  <script>
  import axios from 'axios';
  
  export default {
    data() {
      return {
        headers: [
          { title: 'Company', key: 'company', align:'start', sortable: true },
          { title: 'Role', key: 'role', align:'start', sortable: true  },
          { title: 'Years of Experience', key: 'yearsOfExperience', align:'start', sortable: true },
          { title: 'Salary', key: 'salaries', align:'start', sortable: true },
          { title: 'Yearly Bonus', key: 'yearlyBonus', align:'start', sortable: true },
          { title: 'Actions', key: 'actions', sortable: false, align: 'center'},
        ],
        salaries: [],
        search: '',
        loading: false,
        dialogVisible: false,
        dialogTitle: '',
        form: {
          id: null,
          company: '',
          role: '',
          yearsOfExperience: 0,
          salaries: 0,
          yearlyBonus: null,
        },
        action: '',
        deleteDialog: false,
        selectedItem: null,
        selectedRows: [],
        snackbar: false,
        snackbarText: '',
        snackbarColor: 'info',
      };
    },
    methods: {
      async fetchSalaries(options) {
        this.loading = true;
        try { 
          const response = await axios.get('https://localhost:7188/api/salaries');
          this.salaries = response.data.items || [];
        } catch (error) {
          console.error('Error fetching salaries:', error);
        } finally {
          this.loading = false;
        }
      },
      openDialog(action, item=null) {
        this.action = action;
        this.dialogTitle = action === 'add' || action === 'copy' ? 'Add Salary' : 'Edit Salary';

        if (action === 'edit' && item){
          this.form = { ...item };
        } else if (action === 'copy' && item) {
          this.form = { ...item, id: null };
        } else {
          this.form = {
            id: null,
            company: '',
            role: '',
            yearsOfExperience: 0,
            salaries: 0,
            yearlyBonus: null,
          };
        }
        this.dialogVisible = true;
      },
      copySelectedRow() {
        if (this.selectedRows.length !== 1) {
          this.showSnackbar('Please select only one row to copy', 'error');
          return;
        }

        const selectedItem = this.selectedRows[0];
        this.openDialog('copy', selectedItem);
      },
      showSnackbar(text, color = 'info') {
        this.snackbar = text;
        this.snackbarColor = color;
        this.snackbar = true;
      },
      closeDialog() {
        this.dialogVisible = false;
      },
      async saveSalary() {
        try {
          const dataToSend = {
            ...this.form,
            yearsOfExperience: parseInt(this.form.yearsOfExperience),
            salaries: parseFloat(this.form.salaries),
            yearlyBonus: this.form.yearlyBonus ? parseFloat(this.form.yearlyBonus) : null,
          };

          if (this.action === 'add' || this.action === 'copy'){
            delete dataToSend.id;
          }

          if (this.action === 'add' || this.action === 'copy') {
            await axios.post('https://localhost:7188/api/salaries', dataToSend);
          } else if (this.action === 'edit') {
            await axios.put(`https://localhost:7188/api/salaries/${this.form.id}`, dataToSend);
          }

          this.selectedRows = [];
          this.fetchSalaries();
          this.closeDialog();
          this.showSnackbar('Salary saved succesfully', 'success');          
        } catch (error) {
          console.error('Error saving salary:', error);
          this.showSnackbar('Error saving salary', 'error');
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
  