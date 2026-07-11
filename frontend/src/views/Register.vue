<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { ElForm, ElFormItem, ElInput, ElButton, ElMessage } from 'element-plus'

const router = useRouter()
const userStore = useUserStore()

const form = ref({
  userName: '',
  email: '',
  passWord: ''
})

const loading = ref(false)

const handleRegister = async () => {
  loading.value = true
  try {
    await userStore.registerAction(form.value)
    ElMessage.success('注册成功')
    router.push('/')
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '注册失败')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="register-container">
    <ElForm :model="form" label-width="80px">
      <ElFormItem label="用户名">
        <ElInput v-model="form.userName" placeholder="请输入用户名" />
      </ElFormItem>
      <ElFormItem label="邮箱">
        <ElInput v-model="form.email" placeholder="请输入邮箱" />
      </ElFormItem>
      <ElFormItem label="密码">
        <ElInput v-model="form.passWord" type="password" placeholder="请输入密码" />
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="handleRegister" :loading="loading">注册</ElButton>
      </ElFormItem>
      <ElFormItem>
        <span>已有账号？</span>
        <a href="/login">去登录</a>
      </ElFormItem>
    </ElForm>
  </div>
</template>