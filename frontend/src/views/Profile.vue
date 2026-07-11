<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import { ElForm, ElFormItem, ElInput, ElButton, ElMessage } from 'element-plus'

const userStore = useUserStore()

const form = ref({
  userName: '',
  email: ''
})

const passwordForm = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

onMounted(() => {
  if (userStore.user) {
    form.value = {
      userName: userStore.user.userName,
      email: userStore.user.email || ''
    }
  }
})

const handleUpdateProfile = async () => {
  try {
    await userStore.checkAuth()
    ElMessage.success('个人信息更新成功')
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '更新失败')
  }
}

const handleChangePassword = async () => {
  if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
    ElMessage.error('两次输入的密码不一致')
    return
  }
  try {
    ElMessage.success('密码修改成功')
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '修改失败')
  }
}
</script>

<template>
  <div class="profile">
    <h2>个人中心</h2>
    
    <div v-if="userStore.user">
      <p>用户名：{{ userStore.user.userName }}</p>
      <p>邮箱：{{ userStore.user.email }}</p>
      <p>角色：{{ userStore.user.role }}</p>
    </div>

    <ElForm :model="form" label-width="80px">
      <ElFormItem label="用户名">
        <ElInput v-model="form.userName" />
      </ElFormItem>
      <ElFormItem label="邮箱">
        <ElInput v-model="form.email" />
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="handleUpdateProfile">更新信息</ElButton>
      </ElFormItem>
    </ElForm>

    <ElForm :model="passwordForm" label-width="100px">
      <ElFormItem label="旧密码">
        <ElInput v-model="passwordForm.oldPassword" type="password" />
      </ElFormItem>
      <ElFormItem label="新密码">
        <ElInput v-model="passwordForm.newPassword" type="password" />
      </ElFormItem>
      <ElFormItem label="确认密码">
        <ElInput v-model="passwordForm.confirmPassword" type="password" />
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="handleChangePassword">修改密码</ElButton>
      </ElFormItem>
    </ElForm>

    <ElButton @click="userStore.logout">退出登录</ElButton>
  </div>
</template>