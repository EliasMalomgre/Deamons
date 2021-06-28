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
        ),
        ForeignKey(
            entity = Statement::class,
            parentColumns = ["id"],
            childColumns = ["statementId"],
            onDelete = ForeignKey.CASCADE
        )]
)
data class PartyAnswer(@PrimaryKey val id:Int,
                       var argument:String,
                       val chosenAnswerId:Int,
                       val statementId:Int,
                       var partyName:String
) {



}