<script setup lang="ts">
import { useUserStore } from '@/stores/user'
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElContainer, ElHeader, ElMain, ElMenu, ElMenuItem } from 'element-plus'

const userStore = useUserStore()
const router = useRouter()

onMounted(() => {
  userStore.checkAuth()
})

const handleLogout = () => {
  userStore.logout()
  router.push('/login')
}
</script>

<template>
  <ElContainer class="app-container">
    <ElHeader class="app-header">
      <div class="logo">
        <h2 @click="router.push('/')">我的博客</h2>
      </div>
      <ElMenu mode="horizontal" class="nav-menu">
        <ElMenuItem index="1" @click="router.push('/')">首页</ElMenuItem>
        <template v-if="userStore.isLoggedIn">
          <ElMenuItem index="2" @click="router.push('/post/create')">写文章</ElMenuItem>
          <ElMenuItem index="3" @click="router.push('/profile')">个人中心</ElMenuItem>
          <ElMenuItem index="4" @click="handleLogout">退出登录</ElMenuItem>
        </template>
        <template v-else>
          <ElMenuItem index="5" @click="router.push('/login')">登录</ElMenuItem>
          <ElMenuItem index="6" @click="router.push('/register')">注册</ElMenuItem>
        </template>
      </ElMenu>
    </ElHeader>
    <ElMain class="app-main">
      <router-view />
    </ElMain>
  </ElContainer>
</template>

<style>
.app-container {
  height: 100vh;
}

.app-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.logo h2 {
  margin: 0;
  cursor: pointer;
  font-weight: 600;
}

.nav-menu {
  flex: 1;
  justify-content: flex-end;
}

.app-main {
  padding: 20px;
  background-color: #f5f5f5;
}
</style>