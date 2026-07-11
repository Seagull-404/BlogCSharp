<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElCard, ElInput, ElButton, ElTag, ElRow, ElCol } from 'element-plus'
import { searchPosts, type PostListDto } from '@/api/post'

const router = useRouter()

const articles = ref<PostListDto[]>([])
const keyword = ref('')
const loading = ref(false)

const loadArticles = async () => {
  loading.value = true
  try {
    const result = await searchPosts({ keyword: keyword.value })
    articles.value = result.items || []
  } finally {
    loading.value = false
  }
}

const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleDateString('zh-CN')
}

onMounted(() => {
  loadArticles()
})
</script>

<template>
  <div class="home-container">
    <div class="search-bar">
      <ElInput 
        v-model="keyword" 
        placeholder="搜索文章标题、内容、作者..." 
        size="large"
        class="search-input"
      >
        <template #append>
          <ElButton type="primary" @click="loadArticles" :loading="loading">搜索</ElButton>
        </template>
      </ElInput>
    </div>

    <div class="articles-header">
      <h2>文章列表</h2>
      <span class="article-count">共 {{ articles.length }} 篇文章</span>
    </div>

    <div v-if="articles.length === 0" class="empty-state">
      <ElCard shadow="hover">
        <p class="empty-text">暂无文章</p>
        <p class="empty-hint">快来发布第一篇文章吧！</p>
      </ElCard>
    </div>

    <ElRow :gutter="20" v-else>
      <ElCol :span="8" v-for="article in articles" :key="article.id">
        <ElCard 
          shadow="hover" 
          class="article-card"
          @click="router.push(`/post/${article.id}`)"
        >
          <div class="card-title">{{ article.title }}</div>
          <div class="card-meta">
            <span class="author">作者：{{ article.authorName }}</span>
            <span class="date">{{ formatDate(article.createdAt) }}</span>
          </div>
          <div v-if="article.categoryName" class="card-category">
            <ElTag size="small" type="info">{{ article.categoryName }}</ElTag>
          </div>
        </ElCard>
      </ElCol>
    </ElRow>
  </div>
</template>

<style>
.home-container {
  max-width: 1200px;
  margin: 0 auto;
}

.search-bar {
  margin-bottom: 30px;
}

.search-input {
  width: 100%;
}

.articles-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.articles-header h2 {
  margin: 0;
  color: #303133;
  font-size: 20px;
}

.article-count {
  color: #909399;
  font-size: 14px;
}

.empty-state {
  text-align: center;
  padding: 60px 0;
}

.empty-text {
  font-size: 18px;
  color: #606266;
  margin: 0 0 10px 0;
}

.empty-hint {
  font-size: 14px;
  color: #909399;
  margin: 0;
}

.article-card {
  cursor: pointer;
  transition: transform 0.2s;
}

.article-card:hover {
  transform: translateY(-4px);
}

.card-title {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin-bottom: 12px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.card-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 13px;
  color: #909399;
}

.card-category {
  margin-top: 10px;
}
</style>