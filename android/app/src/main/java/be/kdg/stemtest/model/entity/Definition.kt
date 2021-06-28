package be.kdg.stemtest.model.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity
data class Definition(@PrimaryKey val id:Int,
                        val word:String,
                        val explanation:String)