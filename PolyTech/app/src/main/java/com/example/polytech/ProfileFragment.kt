package com.example.polytech

import android.content.Context
import android.content.Intent
import android.graphics.Bitmap
import android.net.Uri
import android.os.Bundle
import android.provider.MediaStore
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import androidx.fragment.app.Fragment
import com.bumptech.glide.Glide
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity

class ProfileFragment : Fragment() {

    private lateinit var profileImageView: ImageView
    private lateinit var chooseImageButton: Button
    private val PICK_IMAGE_REQUEST = 1

    // Ключ для сохранения URI изображения
    private val PREFS_NAME = "ProfilePrefs"
    private val IMAGE_URI_KEY = "profileImageUri_"

    private var userId: String? = null // Здесь будет ID пользователя

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.fragment_profile, container, false)

        profileImageView = view.findViewById(R.id.profile_image)
        chooseImageButton = view.findViewById(R.id.choose_image_button)

        // Получите ID пользователя (например, после успешной аутентификации)
        userId = getUserId() // Реализуйте этот метод для получения ID пользователя

        // Загрузка сохраненного изображения
        loadSavedImage()

        chooseImageButton.setOnClickListener {
            openGallery()
        }

        return view
    }

    private fun openGallery() {
        val intent = Intent(Intent.ACTION_PICK)
        intent.type = "image/*"
        startActivityForResult(intent, PICK_IMAGE_REQUEST)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == PICK_IMAGE_REQUEST && resultCode == AppCompatActivity.RESULT_OK && data != null) {
            val imageUri: Uri? = data.data
            loadImage(imageUri)
            saveImageUri(imageUri) // Сохранение URI изображения
        }
    }

    private fun loadImage(uri: Uri?) {
        Glide.with(this)
            .load(uri)
            .circleCrop()
            .into(profileImageView)
    }

    private fun saveImageUri(uri: Uri?) {
        val sharedPreferences: SharedPreferences = requireActivity().getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()
        if (uri != null) {
            editor.putString(IMAGE_URI_KEY + userId, uri.toString()) // Сохраняем с уникальным ключом
        }
        editor.apply()
    }

    private fun loadSavedImage() {
        val sharedPreferences: SharedPreferences = requireActivity().getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
        val imageUriString = sharedPreferences.getString(IMAGE_URI_KEY + userId, null)
        if (imageUriString != null) {
            val imageUri = Uri.parse(imageUriString)
            loadImage(imageUri) // Загружаем изображение из сохраненного URI
        }
    }

    private fun getUserId(): String? {
        // Реализуйте вашу логику получения ID пользователя
        return "user@example.com" // Замените на реальный ID пользователя
    }
}