package com.example.polytech

import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.POST

data class ApiRegisterRequest(val name: String,val email: String, val password: String) // Переименовали класс

interface ApiService {
    @POST("api/user") // Путь к вашему API
    fun register(@Body request: ApiRegisterRequest): Call<Void> // Измените Void на нужный тип, если ожидается другой ответ
}
