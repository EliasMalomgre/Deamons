package be.kdg.stemtest.model.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity
data class CustomDebateGameResult(@PrimaryKey  val id : Int,
                                  val values :FloatArray) {
}