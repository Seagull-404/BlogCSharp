<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { ElForm, ElFormItem, ElInput, ElButton, ElMessage, ElCard } from 'element-plus'

const router = useRouter();
const userStore = useUserStore();

const form = ref({
  userName: '',
  passWord: ''
})

const loading = ref(false);

const handleLogin = async () =>
{
  loading.value = true;
  try
  {
    await userStore.loginAction(form.value);
    ElMessage.success('登录成功');
    router.push('/');
    
  }
  catch(error: any)
  {
    ElMessage.error(error.response?.data?.message || '登录失败');
  } finally
  {
    loading.value = false;
  }

}
</script>

<template>
  <div class="login-wrapper">
    <ElCard class="login-card">
      <div class="login-header">
        <h2>欢迎登录</h2>
        <p>请输入您的账号信息</p>
      </div>
      <ElForm :model="form" label-width="80px" class="login-form">
        <ElFormItem label="用户名">
          <ElInput v-model="form.userName" placeholder="请输入用户名" size="large" />
        </ElFormItem>
        <ElFormItem label="密码">
          <ElInput v-model="form.passWord" type="password" placeholder="请输入密码" size="large" />
        </ElFormItem>
        <ElFormItem>
          <ElButton type="primary" @click="handleLogin" :loading="loading" size="large" class="login-btn">登录</ElButton>
        </ElFormItem>
        <ElFormItem class="register-link">
          <span>还没有账号？</span>
          <a href="/register">立即注册</a>
        </ElFormItem>
      </ElForm>
    </ElCard>
  </div>
</template>

<style>
.login-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-card {
  width: 400px;
  padding: 30px;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-header h2 {
  margin: 0 0 10px 0;
  color: #303133;
  font-size: 24px;
}

.login-header p {
  margin: 0;
  color: #909399;
  font-size: 14px;
}

.login-form {
  margin-top: 20px;
}

.login-btn {
  width: 100%;
}

.register-link {
  text-align: center;
  margin-top: 10px;
}

.register-link a {
  color: #667eea;
  text-decoration: none;
  margin-left: 5px;
}

.register-link a:hover {
  text-decoration: underline;
}
</style>