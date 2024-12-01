package com.example.polytech

import android.content.Intent
import android.os.Bundle
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.AppCompatButton
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat

class StartActivity : AppCompatActivity() {

    

    private lateinit var goLoginPage: TextView
    private lateinit var goMainPage: AppCompatButton
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_start)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        goLoginPage = findViewById(R.id.textView2)
        goMainPage =findViewById(R.id.App)

        goLoginPage.setOnClickListener{
            val intent = Intent(this@StartActivity,LoginActivity::class.java)
            startActivity(intent)
            finish()
        }

        goMainPage.setOnClickListener{
            val emailText=findViewById<EditText>(R.id.editText)
            val passwordText = findViewById<EditText>(R.id.editText2)

            val email = emailText.text.toString()
            val password = passwordText.text.toString()

            if(email.isEmpty() || password.isEmpty()){
                Toast.makeText(this@StartActivity,"Пожалуйста, заполните все поля", Toast.LENGTH_SHORT).show() }
            else{
                val intent = Intent(this@StartActivity,MainActivity::class.java)
                startActivity(intent)
            }
        }


    }
}