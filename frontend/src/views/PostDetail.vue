<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getPost, type PostDetailDto } from '@/api/post'
import { ElCard, ElButton, ElTag, ElDivider } from 'element-plus'

const route = useRoute()
const router = useRouter()
const post = ref<PostDetailDto | null>(null)
const loading = ref(true)

const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleString('zh-CN')
}

onMounted(async () => {
  const id = Number(route.params.id)
  try {
    post.value = await getPost(id)
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="post-detail-container" v-if="!loading">
    <div v-if="post" class="post-wrapper">
      <ElCard class="post-card">
        <div class="post-header">
          <ElButton @click="router.push('/')" class="back-btn">← 返回首页</ElButton>
        </div>
        
        <h1 class="post-title">{{ post.title }}</h1>
        
        <div class="post-meta">
          <span class="meta-item">
            <span class="meta-label">作者：</span>
            <span>{{ post.authorName }}</span>
          </span>
          <span class="meta-item">
            <span class="meta-label">分类：</span>
            <ElTag v-if="post.categoryName" type="info">{{ post.categoryName }}</ElTag>
            <span v-else class="no-category">未分类</span>
          </span>
          <span class="meta-item">
            <span class="meta-label">发布时间：</span>
            <span>{{ formatDate(post.createdAt) }}</span>
          </span>
        </div>

        <ElDivider />

        <div v-if="post.tags && post.tags.length > 0" class="post-tags">
          <ElTag 
            v-for="tag in post.tags" 
            :key="tag.id" 
            size="small" 
            class="tag-item"
          >
            {{ tag.name }}
          </ElTag>
        </div>

        <div class="post-content">
          {{ post.content }}
        </div>
      </ElCard>
    </div>

    <div v-else class="not-found">
      <ElCard>
        <h3>文章不存在或已被删除</h3>
        <ElButton @click="router.push('/')">返回首页</ElButton>
      </ElCard>
    </div>
  </div>
</template>

<style>
.post-detail-container {
  max-width: 800px;
  margin: 0 auto;
}

.back-btn {
  margin-bottom: 20px;
}

.post-title {
  font-size: 28px;
  font-weight: 700;
  color: #303133;
  margin: 0 0 20px 0;
  line-height: 1.4;
}

.post-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  margin-bottom: 20px;
  font-size: 14px;
  color: #606266;
}

.meta-item {
  display: flex;
  align-items: center;
}

.meta-label {
  color: #909399;
}

.no-category {
  color: #909399;
  font-style: italic;
}

.post-tags {
  margin-bottom: 20px;
}

.tag-item {
  margin-right: 8px;
}

.post-content {
  font-size: 16px;
  line-height: 1.8;
  color: #303133;
  white-space: pre-wrap;
}

.not-found {
  text-align: center;
  padding: 60px 0;
}
</style>