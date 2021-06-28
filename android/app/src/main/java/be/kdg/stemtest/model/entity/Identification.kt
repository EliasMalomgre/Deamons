package be.kdg.stemtest.model.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity
data class Identification(
    @PrimaryKey  val sessionCode:Int,
    val studentId:Int) {
}