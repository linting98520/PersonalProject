<template>
  <div class="container">
    <h1>人員資料管理</h1>

    <!-- 新增按鈕 -->
    <button @click="openAddForm">新增人員</button>

    <!-- 人員列表 -->
    <table>
      <thead>
        <tr>
          <th>身分證字號</th>
          <th>姓名</th>
          <th>性別</th>
          <th>生日</th>
          <th>縣市</th>
          <th>鄉鎮市區</th>
          <th>地址</th>
          <th>電話</th>
          <th>操作</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="person in persons" :key="person.id">
          <td>{{ person.idNumber }}</td>
          <td>{{ person.name }}</td>
          <td>{{ person.gender }}</td>
          <td>{{ formatDate(person.birthday) }}</td>
          <td>{{ person.city }}</td>
          <td>{{ person.district }}</td>
          <td>{{ person.address }}</td>
          <td>{{ person.phone }}</td>
          <td>
            <button @click="openEditForm(person)">編輯</button>
            <button @click="deletePerson(person.id)">刪除</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- 新增/編輯表單 -->
    <div v-if="showForm" class="modal">
      <div class="modal-content">
        <h2>{{ isEditing ? '編輯人員' : '新增人員' }}</h2>
        <form>
          <label>身分證字號</label>
          <input v-model="form.idNumber" :class="{ 'input-error': errors.IdNumber }" placeholder="A123456789" />
          <span v-if="errors.IdNumber" class="error-msg">{{ errors.IdNumber[0] }}</span>

          <label>姓名</label>
          <input v-model="form.name" :class="{ 'input-error': errors.Name }" />
          <span v-if="errors.Name" class="error-msg">{{ errors.Name[0] }}</span>

          <label>性別</label>
          <select v-model="form.gender" :class="{ 'input-error': errors.Gender }">
            <option value="男">男</option>
            <option value="女">女</option>
          </select>
          <span v-if="errors.Gender" class="error-msg">{{ errors.Gender[0] }}</span>

          <label>生日</label>
          <input type="date" v-model="form.birthday" :class="{ 'input-error': errors.Birthday }" />
          <span v-if="errors.Birthday" class="error-msg">{{ errors.Birthday[0] }}</span>

          <label>縣市</label>
          <input v-model="form.city" :class="{ 'input-error': errors.City }" />
          <span v-if="errors.City" class="error-msg">{{ errors.City[0] }}</span>

          <label>鄉鎮市區</label>
          <input v-model="form.district" :class="{ 'input-error': errors.District }" />
          <span v-if="errors.District" class="error-msg">{{ errors.District[0] }}</span>

          <label>地址</label>
          <input v-model="form.address" :class="{ 'input-error': errors.Address }" />
          <span v-if="errors.Address" class="error-msg">{{ errors.Address[0] }}</span>

          <label>聯絡電話</label>
          <input v-model="form.phone" :class="{ 'input-error': errors.Phone }" />
          <span v-if="errors.Phone" class="error-msg">{{ errors.Phone[0] }}</span>

          <div class="form-buttons">
            <button type="button" @click="submitForm">儲存</button>
            <button type="button" @click="closeForm">取消</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const API_URL = 'http://localhost:5119/api/persons'

const errors = ref({})
const persons = ref([])
const showForm = ref(false)
const isEditing = ref(false)
const form = ref({
  id: null,
  idNumber: '',
  name: '',
  gender: '男',
  birthday: '',
  city: '',
  district: '',
  address: '',
  phone: ''
})

// 取得所有人員
const fetchPersons = async () => {
  const res = await axios.get(API_URL)
  persons.value = res.data
}

// 格式化日期
const formatDate = (date) => {
  return new Date(date).toLocaleDateString('zh-TW')
}

// 開啟新增表單
const openAddForm = () => {
  errors.value = {}
  isEditing.value = false
  form.value = { id: null, idNumber: '', name: '王大明', gender: '男', birthday: '1990-01-01', city: '台北市', district: '信義區', address: '信義路一段', phone: '0912345678' }
  showForm.value = true
}

// 開啟編輯表單
const openEditForm = (person) => {
  errors.value = {}
  isEditing.value = true
  form.value = {
    ...person,
    birthday: person.birthday.substring(0, 10)
  }
  showForm.value = true
}

// 關閉表單
const closeForm = () => {
  showForm.value = false
}

// 儲存（新增或編輯）
const submitForm = async () => {
  errors.value = {}

  const { id, ...rest } = form.value
  const payload = {
    ...rest,
    birthday: form.value.birthday || null,
  }

  console.log('送出的資料：', payload)

  try {
    if (isEditing.value) {
      await axios.put(`${API_URL}/${form.value.id}`, payload)
    } else {
      await axios.post(API_URL, payload)
    }
    closeForm()
    fetchPersons()
  }
  catch (err) {
    console.error('API 失敗', err)

    if (err.response?.data?.errors) {
      errors.value = err.response.data.errors
    } else if (err.response?.data?.message) {
      errors.value = err.response.data.message
      alert(err.response.data.message)
    } else if (err.request) {
      alert(err.request)
    } else {
      alert('儲存失敗，請稍後再試')
    }
  }
}

// 刪除
const deletePerson = async (id) => {
  if (confirm('確定要刪除嗎？')) {
    await axios.delete(`${API_URL}/${id}`)
    fetchPersons()
  }
}

onMounted(() => {
  fetchPersons()
})
</script>

<style scoped>
.container {
  padding: 20px;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

th,
td {
  border: 1px solid #ccc;
  padding: 8px;
  text-align: left;
}

th {
  background-color: #f0f0f0;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-content {
  background: white;
  padding: 30px;
  border-radius: 8px;
  width: 400px;
}

label {
  display: block;
  margin-top: 10px;
}

input,
select {
  width: 100%;
  padding: 6px;
  margin-top: 4px;
  box-sizing: border-box;
}

.form-buttons {
  margin-top: 20px;
  display: flex;
  gap: 10px;
}

.input-error {
  border: 2px solid red;
  outline: none;
}

.error-msg {
  color: red;
  font-size: 12px;
  margin-top: 2px;
  display: block;
}

button {
  padding: 6px 12px;
  cursor: pointer;
}
</style>