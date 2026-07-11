import request from './index'

export interface PostListDto {
  id: number
  title: string
  authorName: string
  categoryName: string
  createdAt: string
}

export interface PostDetailDto {
  id: number
  title: string
  content: string
  authorName: string
  categoryName: string
  tags: { id: number; name: string }[]
  createdAt: string
}

export interface CreatePostDto {
  title: string
  content: string
  categoryId: number | null
  tagIds: number[]
  postStatus: number
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
}

export const getPosts = (params: { pageNumber?: number; pageSize?: number }) => 
  request.get<PagedResult<PostListDto>>('/posts', { params })

export const getPost = (id: number) => request.get<PostDetailDto>(`/posts/${id}`)

export const searchPosts = (params: { keyword?: string; categoryId?: number; tagId?: number; pageNumber?: number; pageSize?: number }) =>
  request.get<PagedResult<PostListDto>>('/posts/search', { params })

export const createPost = (data: CreatePostDto) => request.post<PostDetailDto>('/posts', data)

export const updatePost = (id: number, data: any) => request.put(`/posts/${id}`, data)

export const deletePost = (id: number) => request.delete(`/posts/${id}`)