package com.example.polytech

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.EditText
import android.widget.ImageView
import android.widget.TextView
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.AppCompatButton
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat

class LoginActivity : AppCompatActivity() {

    private lateinit var goMainPage:AppCompatButton
    private lateinit var goStPage: ImageView

    //для валидации данных
    private lateinit var editTextName: EditText
    private lateinit var editTextEmail: EditText
    private lateinit var editTextPassword:EditText

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_login)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        goMainPage = findViewById(R.id.appCompatButton)
        goStPage = findViewById(R.id.imageView3)

        editTextName = findViewById(R.id.editText3)
        editTextEmail = findViewById(R.id.editText4)
        editTextPassword = findViewById(R.id.editText5)

        //поменяла main на mainn тест версия

        goMainPage.setOnClickListener{
            if (validateName() and validateEmail() and validatePassword()){
            val intent = Intent(this@LoginActivity, MainActivity::class.java)
            startActivity(intent)
            finish()}
        }

        goStPage.setOnClickListener{
            val intent = Intent(this@LoginActivity, StartActivity::class.java)
            startActivity(intent)
            finish()
        }
    }

    private fun validateName(): Boolean{
        val name = editTextName.text.toString().trim()
        val tooltipName = findViewById<TextView>(R.id.tooltipName)

        if (name.length !in 2..100){
            tooltipName.visibility=View.VISIBLE //Подсказка
            return false
        }
        tooltipName.visibility=View.INVISIBLE //Скрытие подсказки
        return true
    }

    private fun validateEmail():Boolean{
        val email = editTextEmail.text.toString().trim()
        val tooltipEmail = findViewById<TextView>(R.id.tooltipEmail)

        if(email.length !in 2..100 || "@" !in email){
            tooltipEmail.visibility=View.VISIBLE
            return false
        }
        tooltipEmail.visibility=View.INVISIBLE
        return true
    }

    private fun validatePassword():Boolean{
        val password = editTextPassword.text.toString().trim()
        val tooltipPassword = findViewById<TextView>(R.id.tooltipPassword)

        if(password.length !in 6..100){
            tooltipPassword.visibility=View.VISIBLE
            return false
        }
        tooltipPassword.visibility=View.INVISIBLE
        return true
    }
}