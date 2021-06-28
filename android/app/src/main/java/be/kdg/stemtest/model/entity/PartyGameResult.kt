package be.kdg.stemtest.model.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity
data class PartyGameResult(@PrimaryKey(autoGenerate = true) val id:Int,
                           val score:String)