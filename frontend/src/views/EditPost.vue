<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getPost, updatePost, type PostDetailDto } from '@/api/post'
import request from '@/api/index'
import { ElForm, ElFormItem, ElInput, ElButton, ElMessage, ElSelect, ElOption, ElCard } from 'element-plus'

const route = useRoute()
const router = useRouter()

const post = ref<PostDetailDto | null>(null)
const categories = ref<{ id: number; name: string }[]>([])
const loading = ref(false)

const form = ref({
  title: '',
  content: '',
  categoryId: null as number | null,
  tagIds: [] as number[],
  status: 0
})

const loadCategories = async () => {
  try {
    const res = await request.get<{ id: number; name: string }[]>('/categories')
    categories.value = res ?? []
  } catch {
    // ignore
  }
}

onMounted(async () => {
  loadCategories()
  const id = Number(route.params.id)
  try {
    post.value = await getPost(id)
    if (post.value) {
      form.value = {
        title: post.value.title,
        content: post.value.content,
        categoryId: null,
        tagIds: post.value.tags.map(t => t.id),
        status: 0
      }
    }
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '加载文章失败')
  }
})

const handleSubmit = async () => {
  const id = Number(route.params.id)
  loading.value = true
  try {
    await updatePost(id, form.value)
    ElMessage.success('文章更新成功')
    router.push(`/post/${id}`)
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '更新失败')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="edit-post-container">
    <ElCard class="edit-card">
      <div class="card-header">
        <h2>编辑文章</h2>
        <ElButton @click="router.push(`/post/${route.params.id}`)" class="cancel-btn">取消</ElButton>
      </div>
      
      <ElForm :model="form" label-width="80px" class="edit-form">
        <ElFormItem label="标题" prop="title">
          <ElInput v-model="form.title" placeholder="请输入标题" size="large" />
        </ElFormItem>
        
        <ElFormItem label="分类">
          <ElSelect v-model="form.categoryId" placeholder="不选择分类" clearable>
            <ElOption
              v-for="cat in categories"
              :key="cat.id"
              :label="cat.name"
              :value="cat.id"
            />
          </ElSelect>
        </ElFormItem>
        
        <ElFormItem label="内容" prop="content">
          <ElInput v-model="form.content" type="textarea" placeholder="请输入内容" :rows="15" />
        </ElFormItem>
        
        <ElFormItem>
          <ElButton type="primary" @click="handleSubmit" :loading="loading" size="large">保存修改</ElButton>
          <ElButton @click="router.push(`/post/${route.params.id}`)" style="margin-left: 10px;">返回详情</ElButton>
        </ElFormItem>
      </ElForm>
    </ElCard>
  </div>
</template>

<style>
.edit-post-container {
  max-width: 800px;
  margin: 0 auto;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.card-header h2 {
  margin: 0;
  color: #303133;
}

.cancel-btn {
  margin-bottom: 0;
}

.edit-form {
  margin-top: 20px;
}
</style>