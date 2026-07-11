<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { createPost } from '@/api/post'
import request from '@/api/index'
import { ElForm, ElFormItem, ElInput, ElButton, ElMessage, ElSelect, ElOption } from 'element-plus'

const router = useRouter()

// 分类列表（可选，不选也能发布）
const categories = ref<{ id: number; name: string }[]>([])

const form = ref({
  title: '',
  content: '',
  categoryId: null as number | null,
  tagIds: [] as number[],
  postStatus: 0
})

const loading = ref(false)

// 加载分类列表，供下拉选择
const loadCategories = async () => {
  try {
    const res = await request.get<{ id: number; name: string }[]>('/categories')
    categories.value = res ?? []
  } catch {
    // 分类加载失败不阻塞发布流程
  }
}

const handleSubmit = async () => {
  loading.value = true
  try {
    await createPost(form.value)
    ElMessage.success('文章创建成功')
    router.push('/')
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '创建失败')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadCategories()
})
</script>

<template>
  <div class="create-post">
    <ElForm :model="form" label-width="80px">
      <ElFormItem label="标题">
        <ElInput v-model="form.title" placeholder="请输入标题" />
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
      <ElFormItem label="内容">
        <ElInput v-model="form.content" type="textarea" placeholder="请输入内容" :rows="10" />
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="handleSubmit" :loading="loading">发布</ElButton>
      </ElFormItem>
    </ElForm>
  </div>
</template>