package be.kdg.stemtest.model.entity

import androidx.room.Entity
import androidx.room.ForeignKey
import androidx.room.PrimaryKey

@Entity(
    foreignKeys = [
        ForeignKey(
            entity = Party::class,
            parentColumns = ["name"],
            childColumns = ["partyName"],
            onDelete = ForeignKey.CASCADE
        )]

)
data class DebateGameResult(
    @PrimaryKey val partyName: String,
    val percentage: String
) {
}